namespace Template.Application.Common.Security;

/// <summary>
/// Gerencia contexto de autenticação interna para chamadas de serviços confiáveis.
/// Usa AsyncLocal para garantir isolamento thread-safe.
/// </summary>
public static class InternalAuthContext
{
    private static readonly AsyncLocal<string?> _token = new AsyncLocal<string?>();
    private static readonly AsyncLocal<string?> _userId = new AsyncLocal<string?>();

    // Token secreto gerado aleatoriamente - NUNCA expor externamente
    // Em produção, considere carregar de variável de ambiente
    private const string VALID_TOKEN = "INTERNAL_TRUSTED_SERVICE_TOKEN_aB3dF9gH2jK5mN8pQ1rT4vW7xY0zA";

    /// <summary>
    /// Define o contexto atual como confiável (serviço interno).
    /// Deve ser chamado por serviços internos confiáveis antes de executar Commands/Queries.
    /// </summary>
    public static void SetTrustedContext(Guid? userId = null)
    {
        _token.Value = VALID_TOKEN;
        _userId.Value = userId?.ToString();
    }

    /// <summary>
    /// Obtém o UserId do contexto interno (se definido).
    /// </summary>
    public static string? GetUserId() => _userId.Value;

    /// <summary>
    /// Verifica se o contexto atual é confiável (possui token interno válido).
    /// </summary>
    /// <returns>True se o contexto é confiável, False caso contrário</returns>
    public static bool IsTrusted()
    {
        return _token.Value == VALID_TOKEN;
    }

    /// <summary>
    /// Limpa o token do contexto atual.
    /// SEMPRE deve ser chamado após execução (use try-finally).
    /// </summary>
    public static void Clear()
    {
        _token.Value = null;
        _userId.Value = null;
    }

    /// <summary>
    /// Executa uma ação dentro de um contexto confiável.
    /// Garante que o token seja limpo mesmo em caso de exceção.
    /// </summary>
    /// <param name="action">Ação a ser executada</param>
    /// <param name="userId">UserId opcional para o contexto</param>
    public static void ExecuteAsTrusted(Action action, Guid? userId = null)
    {
        SetTrustedContext(userId);
        try
        {
            action();
        }
        finally
        {
            Clear();
        }
    }

    /// <summary>
    /// Executa uma função assíncrona dentro de um contexto confiável.
    /// Garante que o token seja limpo SOMENTE APÓS a execução completa.
    /// IMPORTANTE: O Clear() só acontece depois do await completar totalmente.
    /// </summary>
    /// <typeparam name="T">Tipo de retorno</typeparam>
    /// <param name="func">Função a ser executada</param>
    /// <param name="userId">UserId para o contexto (obrigatório)</param>
    /// <returns>Resultado da função</returns>
    public static async Task<T> ExecuteAsTrustedAsync<T>(Guid userId, Func<Task<T>> func)
    {
        SetTrustedContext(userId);
        try
        {
            // Aguarda completamente a execução da Task ANTES de limpar o contexto
            var result = await func().ConfigureAwait(false);
            return result;
        }
        finally
        {
            // Clear só é chamado DEPOIS do await completar totalmente
            Clear();
        }
    }

    /// <summary>
    /// Executa uma função assíncrona dentro de um contexto confiável SEM userId.
    /// Garante que o token seja limpo SOMENTE APÓS a execução completa.
    /// </summary>
    /// <typeparam name="T">Tipo de retorno</typeparam>
    /// <param name="func">Função a ser executada</param>
    /// <returns>Resultado da função</returns>
    public static async Task<T> ExecuteAsTrustedAsync<T>(Func<Task<T>> func)
    {
        SetTrustedContext();
        try
        {
            // Aguarda completamente a execução da Task ANTES de limpar o contexto
            var result = await func().ConfigureAwait(false);
            return result;
        }
        finally
        {
            // Clear só é chamado DEPOIS do await completar totalmente
            Clear();
        }
    }
}
