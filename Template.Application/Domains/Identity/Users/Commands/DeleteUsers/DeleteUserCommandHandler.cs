using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;
using Template.Application.Common.Persistence;
using Template.Application.Common.Security;
using Template.Domain.Constants;
using static Template.Domain.Constants.Policies;

namespace Template.Application.Domains.Identity.Users.Commands.DeleteUsers;

[Authorize(Roles = Roles.Admin)]
[Authorize(Policy = $"{CanPurge},{CanManageSettings},{CanManageUsers},{CanAssignPolicies}", PolicyRequirementType = RequirementType.All)]
public class DeleteUserCommandHandler : HandlerBase<Guid, string>
{
    private readonly IIdentityService _identity;

    public DeleteUserCommandHandler(IContext context, HandlerDependencies<Guid, string> dependencies) : base(context, dependencies)
    {
        _identity = dependencies.IdentityService;
    }

    protected override async Task<ApiResponse<string>> RunCore(Guid request, CancellationToken cancellationToken, object? additionalData = null)
    {
        return await _identity.DeleteUserAsync(request.ToString());
    }
}