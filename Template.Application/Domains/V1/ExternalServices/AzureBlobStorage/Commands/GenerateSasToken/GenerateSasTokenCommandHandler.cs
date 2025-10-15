using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Services;
using Template.Application.Common.Models;

namespace Template.Application.Domains.V1.ExternalServices.AzureBlobStorage.Commands.GenerateSasToken;

public class GenerateSasTokenCommandHandler : HandlerBase<GenerateSasTokenCommand, string>
{
    private readonly IAzureStorage _storage;

    public GenerateSasTokenCommandHandler(HandlerDependencies<GenerateSasTokenCommand, string> dependencies, IAzureStorage storage) : base(dependencies)
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