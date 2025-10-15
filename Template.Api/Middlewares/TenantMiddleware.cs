using Microsoft.AspNetCore.Mvc;

namespace Template.Api.Middlewares;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        var endpoint = context.GetEndpoint();

        var groupName = endpoint?.Metadata.GetMetadata<ApiExplorerSettingsAttribute>()?.GroupName;
        context.Items["GroupName"] = groupName;

        await _next(context);
    }
}