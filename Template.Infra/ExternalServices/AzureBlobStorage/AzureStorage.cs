using Azure.Storage.Blobs;
using Template.Application.Common.Interfaces.Services;
using Template.Application.Common.Models;
using Template.Application.ViewModels.Storage;
using Microsoft.AspNetCore.Http;
using Azure.Storage.Sas;
using Azure;
using Azure.Storage.Blobs.Models;

namespace Template.Infra.ExternalServices.AzureBlobStorage;

internal class AzureStorage : IAzureStorage
{
    private readonly BlobContainerClient _client;
    private readonly BlobContainerClient _clientTemp;

    public AzureStorage(BlobContainerClient client, BlobContainerClient clientTemp)
    {
        _client = client;
        _clientTemp = clientTemp;
    }

    public async Task<ApiResponse<UploadFileVM>> UploadFile(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return new ErrorResponse<UploadFileVM>("Arquivo inválido ou vazio.", 400);

        var fileName = Guid.NewGuid();
        var fileExtension = Path.GetExtension(file.FileName);
        var fullFileName = $"{fileName}{fileExtension}";

        try
        {
            var blob = _client.GetBlobClient(fullFileName);
            var response = await blob.UploadAsync(file.OpenReadStream(), cancellationToken);

            if (response.GetRawResponse().Status == 201)
            {
                return new SuccessResponse<UploadFileVM>("Upload realizado com sucesso.",
                    new UploadFileVM(fullFileName, $"{_client.Uri.AbsoluteUri}/{fullFileName}"));
            }
        }
        catch (Exception ex)
        {
            return new ErrorResponse<UploadFileVM>(ex.Message, 500);
        }

        return new ErrorResponse<UploadFileVM>("Falha no upload do arquivo.", 400);
    }

    public string GenerateSasToken(string fileName, int expiracaoMinutos = 5)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new ArgumentException("O nome do arquivo não pode ser vazio.", nameof(fileName));

        try
        {
            BlobClient blob = _clientTemp.GetBlobClient(fileName);

            // Definição das permissões e validade do SAS Token
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _clientTemp.Name,
                BlobName = fileName,
                Resource = "b", // 'b' significa que o token é para um blob específico
                StartsOn = DateTimeOffset.UtcNow, // Disponível imediatamente
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiracaoMinutos)
            };

            // Permissão de escrita (upload)
            sasBuilder.SetPermissions(BlobSasPermissions.Write);

            // Gera a URI do SAS Token
            Uri sasUri = blob.GenerateSasUri(sasBuilder);

            return sasUri.ToString();
        }
        catch (RequestFailedException ex)
        {
            throw new Exception($"Erro ao gerar SAS Token: {ex.Message}", ex);
        }
    }

    public async Task<ApiResponse<string>> MoveFile(string path, string fileName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(fileName))
            return new ErrorResponse<string>("Nome do arquivo não pode ser vazio.", 400);

        try
        {
            var sourceBlob = _clientTemp.GetBlobClient(fileName);
            var destinationBlob = _client.GetBlobClient(path + fileName);

            if (!await sourceBlob.ExistsAsync(cancellationToken))
                return new ErrorResponse<string>("Arquivo não encontrado no container temporário.", 404);

            // Obtém as propriedades do arquivo para verificar o tamanho
            var properties = await sourceBlob.GetPropertiesAsync(cancellationToken: cancellationToken);
            long fileSizeInBytes = properties.Value.ContentLength;
            long maxSize = 5L * 1024 * 1024 * 1024; // 5GB em bytes

            // Inicia a cópia do arquivo
            var copyOperation = await destinationBlob.StartCopyFromUriAsync(sourceBlob.Uri, cancellationToken: cancellationToken);

            // Se o arquivo for até 5GB, aguarda a cópia concluir antes de excluir o original
            if (fileSizeInBytes <= maxSize)
            {
                while (true)
                {
                    var destProperties = await destinationBlob.GetPropertiesAsync(cancellationToken: cancellationToken);
                    if (destProperties.Value.CopyStatus != CopyStatus.Pending)
                        break;
                    await Task.Delay(500, cancellationToken);
                }

                // Exclui o arquivo do container temporário após a cópia bem-sucedida
                await sourceBlob.DeleteIfExistsAsync(cancellationToken: cancellationToken);

                return new SuccessResponse<string>("Arquivo movido com sucesso.");
            }
            else
            {
                return new SuccessResponse<string>("Cópia iniciada para arquivo grande (>5GB). O Azure concluirá em segundo plano.");
            }
        }
        catch (RequestFailedException ex)
        {
            return new ErrorResponse<string>($"Erro ao mover arquivo: {ex.Message}", 500);
        }
        catch (Exception ex)
        {
            return new ErrorResponse<string>(ex.Message, 500);
        }
    }

    public async Task<ApiResponse<string>> DeleteFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return new ErrorResponse<string>("Nome do arquivo não pode ser vazio.", 400);

        try
        {
            var blob = _client.GetBlobClient(fileName);
            var response = await blob.DeleteIfExistsAsync();

            if (response.Value)
                return new SuccessResponse<string>("Arquivo deletado com sucesso.");

            return new ErrorResponse<string>("Arquivo não encontrado no Blob Storage.", 404);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return new ErrorResponse<string>("Arquivo não encontrado no Blob Storage.", 404);
        }
        catch (Exception ex)
        {
            return new ErrorResponse<string>(ex.Message, 500);
        }
    }

    public async Task<ApiResponse<byte[]>> DownloadFile(string fileName)
    {
        try
        {
            var blob = _client.GetBlobClient(fileName);
            var response = await blob.DownloadAsync();

            if (response.GetRawResponse().Status == 200)
            {
                using var memoryStream = new MemoryStream();
                await response.Value.Content.CopyToAsync(memoryStream);
                var content = memoryStream.ToArray();
                return new SuccessResponse<byte[]>("Download efetuado com sucesso.", content);
            }
            else
            {
                return new ErrorResponse<byte[]>("Arquivo não encontrado no Blob Storage.", 404);
            }
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return new ErrorResponse<byte[]>("Arquivo não encontrado no Blob Storage.", 404);
        }
        catch (Exception ex)
        {
            return new ErrorResponse<byte[]>(ex.Message, 500);
        }
    }
}