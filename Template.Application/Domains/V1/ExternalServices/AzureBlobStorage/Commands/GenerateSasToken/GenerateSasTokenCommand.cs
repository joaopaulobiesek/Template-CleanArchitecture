using Template.Application.Common.Security;
using Template.Domain.Constants;

namespace Template.Application.Domains.V1.ExternalServices.AzureBlobStorage.Commands.GenerateSasToken;

//[Authorize(Roles = Roles.Admin)]
//[Authorize(Policy = Policies.CanPurge)]
public class GenerateSasTokenCommand
{
    public string FileName { get; set; }
    public long Size { get; set; }
}