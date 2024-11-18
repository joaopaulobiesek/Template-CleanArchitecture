using Microsoft.Extensions.Hosting;
using System.Reflection;
using Template.Application.Common.Exceptions;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;
using Template.Application.Common.Security;

namespace Template.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> where TResponse : notnull
{
    private readonly ICurrentUser _user;
    private readonly IIdentityService _identityService;
    private readonly IHostEnvironment _environment;

    public AuthorizationBehaviour(ICurrentUser user, IIdentityService identityService, IHostEnvironment environment)
    {
        _user = user;
        _identityService = identityService;
        _environment = environment;
    }

    public async Task<ApiResponse<TResponse>> Handle(Func<Task<ApiResponse<TResponse>>> executeCore, TRequest request, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();
        var missingRoles = new List<string>();

        if (authorizeAttributes.Any())
        {
            if (_user.Id == null)
            {
                throw new UnauthorizedAccessException();
            }

            var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

            if (authorizeAttributesWithRoles.Any())
            {
                var authorized = false;

                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (var role in roles)
                    {
                        var isInRole = await _identityService.IsInRoleAsync(_user.Id, role.Trim());
                        if (isInRole)
                        {
                            authorized = true;
                            break;
                        }
                        else
                        {
                            missingRoles.Add(role.Trim());
                        }
                    }
                }

                if (!authorized)
                {
                    if (_environment.IsDevelopment())
                    {
                        throw new ForbiddenAccessException($"Acesso negado. Faltam as seguintes roles: {string.Join(", ", missingRoles)}");
                    }
                    throw new ForbiddenAccessException();
                }
            }

            var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
            if (authorizeAttributesWithPolicies.Any())
            {
                foreach (var attribute in authorizeAttributesWithPolicies)
                {
                    var policies = attribute.Policy.Split(',').Select(p => p.Trim()).ToList();
                    bool isAuthorized;

                    if (attribute.PolicyRequirementType == RequirementType.All)
                    {
                        isAuthorized = true;
                        foreach (var policy in policies)
                        {
                            var hasPolicy = await _identityService.AuthorizeAsync(_user.Id, policy);
                            if (!hasPolicy)
                            {
                                isAuthorized = false;
                            }
                        }

                        if (!isAuthorized)
                        {
                            if (_environment.IsDevelopment())
                            {
                                throw new ForbiddenAccessException($"Acesso negado. O usuário precisa de TODAS as seguintes policies: {string.Join(", ", policies)}");
                            }
                            throw new ForbiddenAccessException();
                        }
                    }
                    else
                    {
                        isAuthorized = false;
                        foreach (var policy in policies)
                        {
                            var hasPolicy = await _identityService.AuthorizeAsync(_user.Id, policy);
                            if (hasPolicy)
                            {
                                isAuthorized = true;
                                break;
                            }
                        }

                        if (!isAuthorized)
                        {
                            if (_environment.IsDevelopment())
                            {
                                throw new ForbiddenAccessException($"Acesso negado. O usuário precisa de pelo menos UMA das seguintes policies: {string.Join(", ", policies)}");
                            }
                            throw new ForbiddenAccessException();
                        }
                    }
                }
            }
        }

        return await executeCore();
    }
}