using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;
using Template.Application.Common.Security;
using Template.Domain.Constants;
using static Template.Domain.Constants.Policies;

namespace Template.Application.Domains.V1.Identity.Users.Commands.DeleteUsers;

[Authorize(Roles = Roles.Admin)]
[Authorize(Policy = $"{CanPurge},{CanManageSettings},{CanManageUsers},{CanAssignPolicies}", PolicyRequirementType = RequirementType.All)]
public class DeleteUserCommand
{
    public Guid Id{ get; set; }
}

public class DeleteUserCommandHandler : HandlerBase<DeleteUserCommand, string>
{
    private readonly IIdentityService _identity;

    public DeleteUserCommandHandler(HandlerDependencies<DeleteUserCommand, string> dependencies) : base(dependencies)
    {
        _identity = dependencies.IdentityService;
    }

    protected override async Task<ApiResponse<string>> RunCore(DeleteUserCommand request, CancellationToken cancellationToken, object? additionalData = null)
    {
        return await _identity.DeleteUserAsync(request.Id.ToString());
    }
}