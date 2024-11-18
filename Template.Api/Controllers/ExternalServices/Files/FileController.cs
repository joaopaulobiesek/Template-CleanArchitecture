using Template.Api.Controllers.System;
using Template.Application.Common.Interfaces.Services;
using Template.Application.Common.Models;
using Template.Application.Domains.ExternalServices.Storage.Commands.Delete;
using Template.Application.Domains.ExternalServices.Storage.Commands.Upload;
using Template.Application.ViewModels.Storage;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Domains.ExternalServices.Storage.Queries.DownloadFile;

namespace Template.Api.Controllers.ExternalServices.Files;

[ApiController, Route("[controller]")]
public class FileController : BaseController
{
    private Microsoft.Extensions.Primitives.StringValues headerContent;
    /// <summary>
    /// Responsável por Subir arquivo no Azure Storage.
    /// </summary>
    /// <param name="storage"></param>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<UploadFileVM>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<UploadFileVM>))]
    public async Task<IActionResult> UploadFileAsync([FromServices] IStorage storage,
        [FromForm] FileUploadCommand file, CancellationToken cancellationToken)
        => HandleResponse(await storage.UploadFile(file.FormFile, cancellationToken));

    /// <summary>
    /// Responsável por baixar um arquivo no Azure Storage.
    /// </summary>
    /// <param name="storage"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<UploadFileVM>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<UploadFileVM>))]
    public async Task<IActionResult> GetFileAsync([FromServices] IStorage storage,
        [FromBody] DownloadFileQuery file)
        => HandleResponse(await storage.DownloadFile(file.FileName));

    /// <summary>
    /// Responsável por Deletar arquivo no Azure Storage.
    /// </summary>
    /// <param name="storage"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResponse<UploadFileVM>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse<UploadFileVM>))]
    public async Task<IActionResult> DeleteFileAsync([FromServices] IStorage storage,
        [FromBody] FileDeleteCommand file)
        => HandleResponse(await storage.DeleteFile(file.FileName));
}