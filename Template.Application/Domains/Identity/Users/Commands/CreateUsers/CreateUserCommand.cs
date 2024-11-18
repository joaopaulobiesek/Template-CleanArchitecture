using Template.Application.Common.Security;
using static Template.Domain.Constants.Policies;
using static Template.Domain.Constants.Roles;

namespace Template.Application.Domains.Identity.Users.Commands.CreateUsers;

[Authorize(Roles = Admin)]
[Authorize(Policy = $"{CanCreate},{CanManageSettings},{CanManageUsers},{CanAssignPolicies},{CanAssignRoles}", PolicyRequirementType = RequirementType.All)]
public class CreateUserCommand
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public List<string?> Roles { get; set; }
    public List<string?> Policies { get; set; }
    public string? ProfileImageUrl { get; set; }

}