using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.IRepositories.Tenant.Implementations;
using Template.Application.Common.Models;
using Template.Application.Common.Security;
using Template.Domain.Constants;

namespace Template.Application.Domains.Tenant.V1.Clients.Commands.DeleteClient;

[Authorize(Roles = Roles.Admin)]
[Authorize(Policy = Policies.CanPurge)]
public class DeleteClientCommandHandler : HandlerBase<Guid, string>
{
    private readonly IClientRepository _repository;

    public DeleteClientCommandHandler(HandlerDependencies<Guid, string> dependencies, IClientRepository repository) : base(dependencies)
    {
        _repository = repository;
    }

    protected override async Task<ApiResponse<string>> RunCore(Guid request, CancellationToken cancellationToken, object? additionalData = null)
    {
        var client = await _repository.GetByIdAsync(request, cancellationToken);

        if (client == null)
            return new ErrorResponse<string>($"Cliente com o ID '{request}' não encontrado.");

        _repository.Delete(client);

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<string>($"Cliente com o ID '{request}' foi deletado com sucesso!");
    }
}