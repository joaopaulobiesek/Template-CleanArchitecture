using Template.Application.Common.Behaviours;
using Template.Application.Common.Models;
using Template.Application.Common.Security;
using Template.Application.ViewModels.Shared;
using Template.Domain.Constants;
using static Template.Domain.Constants.Policies;

namespace Template.Application.Domains.V1.Identity.Users.Queries.GetPolices;

[Authorize(Roles = Roles.Admin)]
[Authorize(Policy = $"{CanList},{CanView},{CanManageSettings},{CanManageUsers},{CanAssignPolicies}", PolicyRequirementType = RequirementType.All)]
public class GetPolicesQuery
{
}

public class GetPolicesQueryHandler : HandlerBase<GetPolicesQuery, IEnumerable<KeyValuePairVM>>
{
    public GetPolicesQueryHandler(HandlerDependencies<GetPolicesQuery, IEnumerable<KeyValuePairVM>> dependencies) : base(dependencies)
    {
    }

    protected override async Task<ApiResponse<IEnumerable<KeyValuePairVM>>> RunCore(GetPolicesQuery request, CancellationToken cancellationToken, object? additionalData)
        => new SuccessResponse<IEnumerable<KeyValuePairVM>>(
            "Consulta realizada com sucesso",
            GetPolicies().Select(p =>
                new KeyValuePairVM(p.Key, p.Value)
            )
        );
}