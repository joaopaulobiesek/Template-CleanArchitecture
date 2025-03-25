using Microsoft.AspNetCore.Http;
using Template.Application.Common.Security;
using Template.Domain.Constants;

namespace Template.Application.Domains.ExternalServices.AzureBlobStorage.Commands.Upload;

[Authorize(Roles = Roles.Admin)]
[Authorize(Policy = Policies.CanCreate)]
public class FileUploadCommand
{
    public IFormFile FormFile { get; set; }
}