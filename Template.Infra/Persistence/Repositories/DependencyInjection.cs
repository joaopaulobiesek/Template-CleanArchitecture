namespace Template.Infra.Persistence.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        assembly
            .ExportedTypes
            .Where(x => x.IsClass && !x.IsAbstract && x.Name.EndsWith("Repository"))
            .SelectMany(x => x.GetInterfaces(), (c, i) => new { Class = c, Interface = i })
            .Where(tipo => !tipo.Interface.IsGenericType && tipo.Class.Name.StartsWith(tipo.Interface.Name.Substring(1)))
            .ToList()
            .ForEach(tipo => services.AddScoped(tipo.Interface, tipo.Class));

        return services;
    }
}