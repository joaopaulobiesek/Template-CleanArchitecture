using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.IRepositories.Tenant.Implementations;
using Template.Application.Common.Models;
using Template.Application.Common.Security;
using Template.Application.Domains.Tenant.V1.ViewModels;
using Template.Domain.Constants;
using static Template.Domain.Constants.Policies;

namespace Template.Application.Domains.Tenant.V1.Clients.Queries.GetById;

[Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
[Authorize(Policy = $"{CanList},{CanView}")]
public class GetByIdQuery
{
    public Guid Id { get; set; }
}

public class GetByIdQueryHandler : HandlerBase<GetByIdQuery, ClientVM>
{
    private readonly IClientRepository _repository;

    public GetByIdQueryHandler(HandlerDependencies<GetByIdQuery, ClientVM> dependencies, IClientRepository repository) : base(dependencies)
    {
        _repository = repository;
    }

    protected override async Task<ApiResponse<ClientVM>> RunCore(GetByIdQuery request, CancellationToken cancellationToken, object? additionalData = null)
    {
        var client = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (client == null)
            return new ErrorResponse<ClientVM>($"Client with ID '{request.Id}' not found.", 404);

        return new SuccessResponse<ClientVM>("Success", ClientVM.FromEntity(client));
    }
}