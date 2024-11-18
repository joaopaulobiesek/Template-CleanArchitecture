using Template.Application.Common.Security;
using static Template.Domain.Constants.Policies;

namespace Template.Application.Domains.Identity.Users.Commands.EditUsers;

[Authorize(Roles = Domain.Constants.Roles.Admin)]
[Authorize(Policy = $"{CanEdit},{CanManageSettings},{CanManageUsers},{CanAssignPolicies},{CanAssignRoles}", PolicyRequirementType = RequirementType.All)]
public class EditUserCommand
{
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public List<string?> Roles { get; set; }
    public List<string?> Policies { get; set; }
    public string? ProfileImageUrl { get; set; }
}
