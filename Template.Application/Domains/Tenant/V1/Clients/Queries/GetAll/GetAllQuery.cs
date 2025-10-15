using System.Linq.Expressions;
using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.IRepositories.Tenant.Implementations;
using Template.Application.Common.Models;
using Template.Application.Common.Security;
using Template.Application.Domains.Tenant.V1.ViewModels;
using Template.Domain.Constants;
using Template.Domain.Entity.Tenant;
using static Template.Domain.Constants.Policies;

namespace Template.Application.Domains.Tenant.V1.Clients.Queries.GetAll;

[Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
[Authorize(Policy = $"{CanList},{CanView}")]
public class GetAllQuery
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int AscDesc { get; set; }
    public string? ColumnName { get; set; }
    public string? SearchText { get; set; }
}

public class GetAllQueryHandler : HandlerBase<GetAllQuery, IEnumerable<ClientVM>>
{
    private readonly IClientRepository _repository;

    public GetAllQueryHandler(HandlerDependencies<GetAllQuery, IEnumerable<ClientVM>> dependencies, IClientRepository repository) : base(dependencies)
    {
        _repository = repository;
    }

    protected override async Task<ApiResponse<IEnumerable<ClientVM>>> RunCore(GetAllQuery request, CancellationToken cancellationToken, object? additionalData = null)
    {
        var query = _repository.SearchIQueryable(request.SearchText!);

        query = request.AscDesc == -1
            ? query.OrderByDescending(BuscarOrdemPropriedade(request.ColumnName))
            : query.OrderBy(BuscarOrdemPropriedade(request.ColumnName));

        var list = await PaginatedList<Client>.CreateAsync(query, request.PageNumber, request.PageSize, cancellationToken);

        var newList = list.Data?.Select(x => new ClientVM(x.Id, x.FullName, x.DocumentNumber, x.Phone, x.ZipCode, x.Paid)).ToList();

        var result = new PaginatedList<ClientVM>(
                newList,
                list.TotalItens,
                list.PageNumber,
                request.PageSize
            );

        return result;
    }

    private static Expression<Func<Client, object>> BuscarOrdemPropriedade(string? param)
     => param?.ToLower() switch
     {
         "document" => Client => Client.DocumentNumber,
         _ => Client => Client.FullName
     };
}