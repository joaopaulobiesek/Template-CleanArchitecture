
using Template.Application.Common.Exceptions;
using Template.Application.Common.Interfaces.IRepositories.Tenant.Implementations;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;
using Template.Application.Common.Modules;
using Microsoft.Extensions.Hosting;

namespace Template.Application.Common.Behaviours;

public class ModuleValidationBehaviour<TRequest, TResponse> where TResponse : notnull
{
    private readonly IClientRepository _client;
    private readonly IHostEnvironment _env;
    private readonly ICurrentUser _user;

    public ModuleValidationBehaviour(IClientRepository client, IHostEnvironment env, ICurrentUser user)
    {
        _client = client;
        _env = env;
        _user = user;
    }

    public async Task<ApiResponse<TResponse>> Handle(Func<Task<ApiResponse<TResponse>>> next, TRequest request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_user.GroupName) && _user.GroupName.Contains("Tenant.Api."))
        {
            return await next();
        }

        var rawAttributes = request.GetType().GetCustomAttributes(typeof(RequiresModuleAttribute), true);

        var requiredModules = rawAttributes.Cast<RequiresModuleAttribute>();

        if (requiredModules.Any())
        {
            var availableModules = await _client.GetActiveModulesAsync(_user.Id!);

            foreach (var module in requiredModules)
            {
                if (!availableModules.Contains(module.Module))
                {
                    if (_env.IsDevelopment())
                        throw new ForbiddenAccessException($"Module '{module.Module}' is not enabled for this tenant.");

                    throw new ForbiddenAccessException("Module is not enabled for this tenant.");
                }
            }
        }

        return await next();
    }
}