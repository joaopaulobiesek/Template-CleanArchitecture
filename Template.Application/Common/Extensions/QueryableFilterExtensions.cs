using System.Linq.Expressions;
using System.Reflection;

namespace Template.Application.Common.Extensions;

public static class QueryableFilterExtensions
{
    /// <summary>
    /// Aplica filtros dinâmicos baseados em um dicionário de propriedade/valor
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    /// <param name="query">Query base</param>
    /// <param name="customFilter">Dicionário com nome da propriedade e valor</param>
    /// <returns>Query filtrada</returns>
    public static IQueryable<T> ApplyCustomFilters<T>(
        this IQueryable<T> query,
        Dictionary<string, string>? customFilter) where T : class
    {
        if (customFilter == null || !customFilter.Any())
            return query;

        foreach (var filter in customFilter)
        {
            query = ApplyFilter(query, filter.Key, filter.Value);
        }

        return query;
    }

    /// <summary>
    /// Aplica filtros dinâmicos COM WHITELIST de propriedades permitidas
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    /// <param name="query">Query base</param>
    /// <param name="customFilter">Dicionário com nome da propriedade e valor</param>
    /// <param name="allowedProperties">Array de nomes de propriedades permitidas (case insensitive)</param>
    /// <returns>Query filtrada ou null se houver propriedade não permitida</returns>
    public static IQueryable<T>? ApplyCustomFiltersWithWhitelist<T>(
        this IQueryable<T> query,
        Dictionary<string, string>? customFilter,
        params string[] allowedProperties) where T : class
    {
        if (customFilter == null || !customFilter.Any())
            return query;

        // Converte whitelist para lowercase para comparação case-insensitive
        var allowedPropsLower = allowedProperties.Select(p => p.ToLower()).ToHashSet();

        // Valida se TODAS as propriedades do filtro estão na whitelist
        foreach (var filter in customFilter)
        {
            if (!allowedPropsLower.Contains(filter.Key.ToLower()))
            {
                // Propriedade NÃO permitida! Retorna null
                return null;
            }
        }

        // Se passou na validação, aplica os filtros
        foreach (var filter in customFilter)
        {
            query = ApplyFilter(query, filter.Key, filter.Value);
        }

        return query;
    }

    private static IQueryable<T> ApplyFilter<T>(
        IQueryable<T> query,
        string propertyName,
        string value) where T : class
    {
        if (string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrWhiteSpace(value))
            return query;

        // Get property info (case insensitive)
        var property = typeof(T).GetProperty(
            propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null)
            return query; // Propriedade não existe, ignora filtro

        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.Property(parameter, property);

        Expression? filterExpression = null;

        // Trata diferentes tipos de propriedade
        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        if (propertyType == typeof(string))
        {
            filterExpression = BuildStringContainsExpression(propertyAccess, value);
        }
        else if (propertyType == typeof(DateTime))
        {
            if (DateTime.TryParse(value, out var dateValue))
            {
                filterExpression = BuildComparisonExpression(propertyAccess, dateValue, propertyName);
            }
        }
        else if (propertyType == typeof(Guid))
        {
            if (Guid.TryParse(value, out var guidValue))
            {
                var constant = Expression.Constant(guidValue, property.PropertyType);
                filterExpression = Expression.Equal(propertyAccess, constant);
            }
        }
        else if (propertyType == typeof(int) || propertyType == typeof(long) ||
                 propertyType == typeof(decimal) || propertyType == typeof(double))
        {
            filterExpression = BuildNumericExpression(propertyAccess, value, propertyType, propertyName);
        }
        else if (propertyType.IsEnum)
        {
            if (Enum.TryParse(propertyType, value, true, out var enumValue))
            {
                var constant = Expression.Constant(enumValue, property.PropertyType);
                filterExpression = Expression.Equal(propertyAccess, constant);
            }
        }
        else if (propertyType == typeof(bool))
        {
            if (bool.TryParse(value, out var boolValue))
            {
                var constant = Expression.Constant(boolValue, property.PropertyType);
                filterExpression = Expression.Equal(propertyAccess, constant);
            }
        }

        if (filterExpression != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
            query = query.Where(lambda);
        }

        return query;
    }

    /// <summary>
    /// Constrói expressão para strings usando Contains (case insensitive)
    /// </summary>
    private static Expression BuildStringContainsExpression(Expression propertyAccess, string value)
    {
        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;

        // Verifica se não é null
        var notNullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));

        // x.Property.ToLower().Contains(value.ToLower())
        var toLower = Expression.Call(propertyAccess, toLowerMethod);
        var valueLower = Expression.Constant(value.ToLower(), typeof(string));
        var contains = Expression.Call(toLower, containsMethod, valueLower);

        // x.Property != null && x.Property.ToLower().Contains(value.ToLower())
        return Expression.AndAlso(notNullCheck, contains);
    }

    /// <summary>
    /// Constrói expressão de comparação para datas (suporta operadores)
    /// Exemplos: ">=2024-01-01", "<=2024-12-31", "2024-06-15"
    /// </summary>
    private static Expression BuildComparisonExpression(
        Expression propertyAccess,
        DateTime dateValue,
        string propertyName)
    {
        // Suporta startDate/endDate ou operadores explícitos
        if (propertyName.EndsWith("StartDate", StringComparison.OrdinalIgnoreCase) ||
            propertyName.StartsWith("Start", StringComparison.OrdinalIgnoreCase))
        {
            var constant = Expression.Constant(dateValue, propertyAccess.Type);
            return Expression.GreaterThanOrEqual(propertyAccess, constant);
        }

        if (propertyName.EndsWith("EndDate", StringComparison.OrdinalIgnoreCase) ||
            propertyName.StartsWith("End", StringComparison.OrdinalIgnoreCase))
        {
            var constant = Expression.Constant(dateValue, propertyAccess.Type);
            return Expression.LessThanOrEqual(propertyAccess, constant);
        }

        // Igualdade por padrão
        var defaultConstant = Expression.Constant(dateValue, propertyAccess.Type);
        return Expression.Equal(propertyAccess, defaultConstant);
    }

    /// <summary>
    /// Constrói expressão numérica (suporta operadores: =, >, <, >=, <=)
    /// Exemplos: ">=100", "<=50", "42"
    /// </summary>
    private static Expression? BuildNumericExpression(
        Expression propertyAccess,
        string value,
        Type propertyType,
        string propertyName)
    {
        // Remove espaços
        value = value.Trim();

        // Detecta operador
        string op = "=";
        string numericValue = value;

        if (value.StartsWith(">="))
        {
            op = ">=";
            numericValue = value.Substring(2).Trim();
        }
        else if (value.StartsWith("<="))
        {
            op = "<=";
            numericValue = value.Substring(2).Trim();
        }
        else if (value.StartsWith(">"))
        {
            op = ">";
            numericValue = value.Substring(1).Trim();
        }
        else if (value.StartsWith("<"))
        {
            op = "<";
            numericValue = value.Substring(1).Trim();
        }

        // Converte valor
        object? parsedValue = null;

        if (propertyType == typeof(int) && int.TryParse(numericValue, out var intValue))
            parsedValue = intValue;
        else if (propertyType == typeof(long) && long.TryParse(numericValue, out var longValue))
            parsedValue = longValue;
        else if (propertyType == typeof(decimal) && decimal.TryParse(numericValue, out var decimalValue))
            parsedValue = decimalValue;
        else if (propertyType == typeof(double) && double.TryParse(numericValue, out var doubleValue))
            parsedValue = doubleValue;

        if (parsedValue == null)
            return null;

        var constant = Expression.Constant(parsedValue, propertyAccess.Type);

        return op switch
        {
            ">=" => Expression.GreaterThanOrEqual(propertyAccess, constant),
            "<=" => Expression.LessThanOrEqual(propertyAccess, constant),
            ">" => Expression.GreaterThan(propertyAccess, constant),
            "<" => Expression.LessThan(propertyAccess, constant),
            _ => Expression.Equal(propertyAccess, constant)
        };
    }
}
