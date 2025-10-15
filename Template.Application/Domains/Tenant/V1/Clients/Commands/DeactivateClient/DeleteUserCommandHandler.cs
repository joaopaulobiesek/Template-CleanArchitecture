using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.IRepositories.Tenant.Implementations;
using Template.Application.Common.Models;
using Template.Application.Common.Security;
using Template.Domain.Constants;

namespace Template.Application.Domains.Tenant.V1.Clients.Commands.DeactivateClient;

[Authorize(Roles = Roles.Admin)]
[Authorize(Policy = Policies.CanDelete)]
public class DeactivateClientCommand
{
    public Guid Id { get; set; }
}

public class DeactivateClientCommandHandler : HandlerBase<DeactivateClientCommand, string>
{
    private readonly IClientRepository _repository;

    public DeactivateClientCommandHandler(HandlerDependencies<DeactivateClientCommand, string> dependencies, IClientRepository repository) : base(dependencies)
    {
        _repository = repository;
    }

    protected override async Task<ApiResponse<string>> RunCore(DeactivateClientCommand request, CancellationToken cancellationToken, object? additionalData = null)
    {
        var client = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (client == null)
            return new ErrorResponse<string>($"Cliente com o ID '{request.Id}' não encontrado.");

        client.Inactivate();

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Cliente com o ID '{request.Id}' foi desativado com sucesso!");
    }
}