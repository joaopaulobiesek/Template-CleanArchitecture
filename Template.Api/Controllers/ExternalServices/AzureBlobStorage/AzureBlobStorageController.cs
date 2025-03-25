using Template.Api.Controllers.System;
using Template.Application.Common.Behaviours;
using Template.Application.Common.Interfaces.Services;
using Template.Application.Common.Models;
using Template.Application.Domains.ExternalServices.AzureBlobStorage.Commands.Delete;
using Template.Application.Domains.ExternalServices.AzureBlobStorage.Commands.GenerateSasToken;
using Template.Application.Domains.ExternalServices.AzureBlobStorage.Commands.Upload;
using Template.Application.Domains.ExternalServices.AzureBlobStorage.Queries.DownloadFile;
using Microsoft.AspNetCore.Mvc;
using Template.Application.ViewModels.Storage;

namespace Template.Api.Controllers.ExternalServices.AzureBlobStorage;

[ApiController]
[Route("api/v1/[controller]")]
public class AzureBlobStorageController : BaseController
{
    private Microsoft.Extensions.Primitives.StringValues headerContent;

    /// <summary>
    /// Gera um SAS Token do Azure Blob Storage para upload de arquivos.
    /// </summary>
    /// <remarks>
    /// Este endpoint gera uma URL assinada (SAS Token) que permite o upload seguro de arquivos
    /// diretamente para o Azure Blob Storage, sem expor credenciais sensíveis.
    ///
    /// **Regras de negócio:**
    /// - O usuário deve estar autenticado.
    /// - O nome do arquivo deve ser informado no corpo da requisição.
    /// - O token gerado será válido apenas por um período limitado.
    /// - O upload deve ser feito utilizando a URL gerada, respeitando as permissões concedidas no SAS Token.
    ///
    /// **Fluxo de uso:**
    /// 1. O cliente solicita a geração do SAS Token informando o nome do arquivo.
    /// 2. O backend retorna uma URL assinada contendo o token de acesso temporário.
    /// 3. O cliente faz o upload do arquivo diretamente para o Azure Blob Storage usando a URL gerada.
    ///
    /// **Observações:**
    /// - O SAS Token gerado terá permissões apenas de **gravação** (Write).
    /// - O tempo de expiração do token pode ser configurado conforme necessário.
    ///
    /// </remarks>
    /// <param name="handler">Handler responsável pela execução da geração do token.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Uma URL assinada contendo o SAS Token.</returns>
    /// <response code="200">SAS Token gerado com sucesso.</response>
    /// <response code="400">Parâmetros inválidos ou erro na requisição.</response>
    /// <response code="401">O usuário não está autenticado.</response>
    /// <response code="403">O usuário não tem permissão para acessar este recurso.</response>
    [HttpPost("GenerateSasToken")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<string>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse<string>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> GenerateSasTokenAsync([FromServices] IHandlerBase<GenerateSasTokenCommand, string> handler,
        [FromBody] GenerateSasTokenCommand command, CancellationToken cancellationToken)
        => HandleResponse(await handler.Execute(command, cancellationToken));

    /// <summary>
    /// Faz o upload de um arquivo para o Azure Blob Storage.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite que o usuário envie um arquivo para ser armazenado no Azure Blob Storage.
    /// O arquivo será salvo com um nome único gerado automaticamente para evitar conflitos.
    ///
    /// **Regras de negócio:**
    /// - O usuário deve estar autenticado.
    /// - O arquivo deve ser enviado no formato `multipart/form-data`.
    /// - O sistema gera um nome único para o arquivo antes de armazená-lo.
    /// - Apenas arquivos com extensões permitidas podem ser armazenados.
    ///
    /// **Fluxo de uso:**
    /// 1. O cliente faz um `POST` enviando o arquivo via `multipart/form-data`.
    /// 2. O backend recebe o arquivo e gera um nome único para ele.
    /// 3. O arquivo é enviado para o Azure Blob Storage.
    /// 4. Se o upload for bem-sucedido, o backend retorna a URL do arquivo armazenado.
    ///
    /// **Observações:**
    /// - O nome do arquivo original será substituído por um GUID para evitar duplicações.
    /// - O sistema pode validar extensões ou tamanhos antes de permitir o upload.
    /// - A URL retornada pode ser utilizada para acessar o arquivo posteriormente.
    ///
    /// </remarks>
    /// <param name="handler">Serviço de armazenamento responsável pelo upload.</param>
    /// <param name="file">Arquivo a ser enviado.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Informações sobre o arquivo armazenado.</returns>
    /// <response code="201">Upload realizado com sucesso.</response>
    /// <response code="400">Parâmetros inválidos ou erro na requisição.</response>
    /// <response code="401">O usuário não está autenticado.</response>
    /// <response code="403">O usuário não tem permissão para acessar este recurso.</response>
    [HttpPost("UploadFile")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResponse<UploadFileVM>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<UploadFileVM>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse<UploadFileVM>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse<UploadFileVM>))]
    public async Task<IActionResult> UploadFileAsync([FromServices] IAzureStorage handler,
    [FromForm] FileUploadCommand file, CancellationToken cancellationToken)
    => HandleResponse(await handler.UploadFile(file.FormFile, cancellationToken));

    /// <summary>
    /// Baixa um arquivo armazenado no Azure Blob Storage.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite que o usuário faça o download de um arquivo armazenado no Azure Blob Storage.
    /// O arquivo é recuperado com base no nome informado na requisição.
    ///
    /// **Regras de negócio:**
    /// - O usuário deve estar autenticado.
    /// - O nome do arquivo deve ser informado no corpo da requisição.
    /// - O sistema verifica se o arquivo existe antes de realizar o download.
    /// - Se o arquivo não for encontrado, será retornado um erro apropriado.
    ///
    /// **Fluxo de uso:**
    /// 1. O cliente informa o nome do arquivo a ser baixado.
    /// 2. O backend busca o arquivo no Azure Blob Storage.
    /// 3. Se encontrado, o arquivo é retornado em formato binário.
    /// 4. Se o arquivo não existir, uma resposta de erro será retornada.
    ///
    /// **Observações:**
    /// - O arquivo será retornado como um array de bytes (`byte[]`).
    /// - Caso o arquivo não exista ou ocorra um erro, o sistema retornará uma mensagem de erro apropriada.
    ///
    /// </remarks>
    /// <param name="handler">Serviço de armazenamento responsável por buscar o arquivo.</param>
    /// <param name="file">Objeto contendo o nome do arquivo a ser baixado.</param>
    /// <returns>O arquivo solicitado em formato binário.</returns>
    /// <response code="200">Download efetuado com sucesso.</response>
    /// <response code="400">Parâmetros inválidos ou erro na requisição.</response>
    /// <response code="404">Arquivo não encontrado no Blob Storage.</response>
    /// <response code="401">O usuário não está autenticado.</response>
    /// <response code="403">O usuário não tem permissão para acessar este recurso.</response>
    [HttpGet("DownloadFile")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<byte[]>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<byte[]>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse<byte[]>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse<byte[]>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse<byte[]>))]
    public async Task<IActionResult> GetFileAsync([FromServices] IAzureStorage handler,
        [FromQuery] DownloadFileQuery file, CancellationToken cancellationToken)
        => HandleResponse(await handler.DownloadFile(file.FileName));

    /// <summary>
    /// Exclui um arquivo armazenado no Azure Blob Storage.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite que o usuário exclua um arquivo armazenado no Azure Blob Storage.
    /// O arquivo é identificado pelo nome informado na requisição.
    ///
    /// **Regras de negócio:**
    /// - O usuário deve estar autenticado.
    /// - O nome do arquivo deve ser informado na requisição.
    /// - Se o arquivo não for encontrado, uma mensagem de erro será retornada.
    ///
    /// **Fluxo de uso:**
    /// 1. O cliente informa o nome do arquivo a ser excluído.
    /// 2. O backend verifica se o arquivo existe no Azure Blob Storage.
    /// 3. Se encontrado, o arquivo é removido.
    /// 4. Se o arquivo não existir, uma resposta de erro apropriada será retornada.
    ///
    /// **Observações:**
    /// - O sistema não armazena versões dos arquivos excluídos.
    /// - A exclusão é permanente, sem possibilidade de recuperação.
    /// - Caso o arquivo não exista, será retornada uma mensagem de erro apropriada.
    ///
    /// </remarks>
    /// <param name="handler">Serviço de armazenamento responsável pela exclusão do arquivo.</param>
    /// <param name="fileName">Nome do arquivo a ser excluído.</param>
    /// <returns>Mensagem indicando o sucesso ou erro da operação.</returns>
    /// <response code="200">Arquivo deletado com sucesso.</response>
    /// <response code="400">Parâmetros inválidos ou erro na requisição.</response>
    /// <response code="404">Arquivo não encontrado no Blob Storage.</response>
    /// <response code="401">O usuário não está autenticado.</response>
    /// <response code="403">O usuário não tem permissão para acessar este recurso.</response>
    [HttpDelete("DeleteFile")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponse<string>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ErrorResponse<string>))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ErrorResponse<string>))]
    public async Task<IActionResult> DeleteFileAsync([FromServices] IAzureStorage handler,
        [FromQuery] FileDeleteCommand file, CancellationToken cancellationToken)
        => HandleResponse(await handler.DeleteFile(file.FileName));
}