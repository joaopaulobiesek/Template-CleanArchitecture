using Template.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Template.Application.Domains.V1.ViewModels.Storage;

namespace Template.Application.Common.Interfaces.Services;

public interface IAzureStorage
{
    Task<ApiResponse<UploadFileVM>> UploadFile(IFormFile file, CancellationToken cancellationToken);
    string GenerateSasToken(string fileName, int expiracaoMinutos = 5);
    Task<ApiResponse<string>> MoveFile(string path, string fileName, CancellationToken cancellationToken);
    Task<ApiResponse<string>> DeleteFile(string fileName);
    Task<ApiResponse<byte[]>> DownloadFile(string fileName);
}