using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;
using Template.Application.Common.Security;
using Template.Application.Domains.V1.ViewModels.Users;
using Template.Domain.Constants;
using static Template.Domain.Constants.Policies;

namespace Template.Application.Domains.V1.Identity.Users.Queries.GetAll;

[Authorize(Roles = Roles.Admin)]
[Authorize(Policy = $"{CanList},{CanView},{CanManageSettings},{CanManageUsers}", PolicyRequirementType = RequirementType.All)]
public class GetAllQuery
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int AscDesc { get; set; }
    public string? ColumnName { get; set; }
    public string? SearchText { get; set; }
}

public class GetAllQueryHandler : HandlerBase<GetAllQuery, IEnumerable<UserVm>>
{
    private readonly IIdentityService _identity;

    public GetAllQueryHandler(HandlerDependencies<GetAllQuery, IEnumerable<UserVm>> dependencies) : base(dependencies)
    {
        _identity = dependencies.IdentityService;
    }

    protected override async Task<ApiResponse<IEnumerable<UserVm>>> RunCore(GetAllQuery request, CancellationToken cancellationToken, object? additionalData)
    {
        var list = await PaginatedList<UserVm>.CreateAsync(
            _identity.ListUsersAsync(request.AscDesc, request.ColumnName!, request.SearchText)!,
            request.PageNumber,
            request.PageSize,
            cancellationToken
        );

        if (list.Data == null || !list.Data.Any())
            return new SuccessResponse<IEnumerable<UserVm>>("Consulta conclu�da com sucesso, por�m nenhum registro foi encontrado.", null);

        foreach (var user in list.Data)
        {
            user.Roles = await _identity.GetUserRole(user.Id!);
            user.Policies = await _identity.GetUserPolicies(user.Id!);
        }

        return list;
    }
}