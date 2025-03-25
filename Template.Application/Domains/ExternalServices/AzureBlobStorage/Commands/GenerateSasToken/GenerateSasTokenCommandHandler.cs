using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Services;
using Template.Application.Common.Models;
using Template.Application.Common.Persistence;

namespace Template.Application.Domains.ExternalServices.AzureBlobStorage.Commands.GenerateSasToken;

public class GenerateSasTokenCommandHandler : HandlerBase<GenerateSasTokenCommand, string>
{
    private readonly IAzureStorage _storage;

    public GenerateSasTokenCommandHandler(IContext context, HandlerDependencies<GenerateSasTokenCommand, string> dependencies, IAzureStorage storage) : base(context, dependencies)
    {
        _storage = storage;
    }

    protected override async Task<ApiResponse<string>> RunCore(GenerateSasTokenCommand request, CancellationToken cancellationToken, object? additionalData = null)
    {
        //TODO: Implementar a variação de tempo conforme o tamanho do arquivo.

        var result = _storage.GenerateSasToken(request.FileName);

        return new SuccessResponse<string>("Token Gerado com Sucesso!", result);
    }
}