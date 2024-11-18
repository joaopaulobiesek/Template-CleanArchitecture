using Template.Application.Common.Models;
using Template.Application.ViewModels.Storage;
using Microsoft.AspNetCore.Http;

namespace Template.Application.Common.Interfaces.Services;

public interface IStorage
{
    Task<ApiResponse<UploadFileVM>> UploadFile(IFormFile file, CancellationToken cancellationToken);
    Task<ApiResponse<UploadFileVM>> DeleteFile(string fileName);
    Task<ApiResponse<byte[]>> DownloadFile(string fileName);
}