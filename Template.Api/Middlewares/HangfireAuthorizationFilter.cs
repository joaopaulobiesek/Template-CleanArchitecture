using Hangfire.Dashboard;

namespace Template.Api.Middlewares;

/// <summary>
/// Filtro de autorização para Hangfire Dashboard.
/// Permite acesso apenas para usuários autenticados com role ADMIN.
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Verifica se o usuário está autenticado
        if (httpContext.User?.Identity?.IsAuthenticated != true)
            return false;

        // Permite acesso apenas para usuários com role ADMIN
        return httpContext.User.IsInRole("ADMIN");
    }
}