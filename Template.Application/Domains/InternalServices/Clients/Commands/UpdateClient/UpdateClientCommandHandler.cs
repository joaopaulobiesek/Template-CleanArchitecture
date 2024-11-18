using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.IRepositories.Implementations;
using Template.Application.Common.Models;
using Template.Application.Common.Persistence;
using Template.Application.ViewModels.Clients;

namespace Template.Application.Domains.InternalServices.Clients.Commands.UpdateClient;

public class UpdateClientCommandHandler : HandlerBase<UpdateClientCommand, ClientVM>
{
    private readonly IClientRepository _repository;
    public UpdateClientCommandHandler(IContext context, HandlerDependencies<UpdateClientCommand, ClientVM> dependencies, IClientRepository repository) : base(context, dependencies)
    {
        _repository = repository;
    }

    protected override async Task<ApiResponse<ClientVM>> RunCore(UpdateClientCommand request, CancellationToken cancellationToken, object? additionalData = null)
    {
        var client = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (client == null)
            return new ErrorResponse<ClientVM>($"Cliente com o ID '{request.Id}' não encontrado.");

        client.UpdateClient(request);

        _context.Clients.Update(client);

        //_repository.Update(client);

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResponse<ClientVM>(
            "Cadastro efetuado com sucesso.",
            new ClientVM(
                client.Id,
                client.FullName,
                client.DocumentNumber,
                client.Phone,
                client.ZipCode
            )
        );
    }
}