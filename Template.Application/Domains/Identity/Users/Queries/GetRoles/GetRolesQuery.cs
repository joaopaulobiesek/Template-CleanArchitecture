using Template.Application.Common.Behaviours;
using Template.Application.Common.Models;
using Template.Application.Common.Persistence;
using Template.Application.Common.Security;
using Template.Application.ViewModels.Shared;
using Template.Domain.Constants;
using static Template.Domain.Constants.Policies;

namespace Template.Application.Domains.Identity.Users.Queries.GetRoles;

[Authorize(Roles = Roles.Admin)]
[Authorize(Policy = $"{CanList},{CanView},{CanManageSettings},{CanManageUsers},{CanAssignRoles}", PolicyRequirementType = RequirementType.All)]
public class GetRolesQuery
{
}

public class GetRolesQueryHandler : HandlerBase<GetRolesQuery, IEnumerable<KeyValuePairVM>>
{
    public GetRolesQueryHandler(IContext context, HandlerDependencies<GetRolesQuery, IEnumerable<KeyValuePairVM>> dependencies) : base(context, dependencies)
    {
    }

    protected override async Task<ApiResponse<IEnumerable<KeyValuePairVM>>> RunCore(GetRolesQuery request, CancellationToken cancellationToken, object? additionalData)
        => new SuccessResponse<IEnumerable<KeyValuePairVM>>(
            "Consulta realizada com sucesso",
            Roles.GetRoles().Select(p =>
            new KeyValuePairVM(p.Key, p.Value)
            )
        );
}