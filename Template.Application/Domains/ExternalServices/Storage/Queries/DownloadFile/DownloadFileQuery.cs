using Template.Application.Common.Security;
using Template.Domain.Constants;

namespace Template.Application.Domains.ExternalServices.Storage.Queries.DownloadFile;

[Authorize(Roles = Roles.Admin)]
[Authorize(Policy = Policies.CanView)]
public class DownloadFileQuery
{
    public string FileName { get; set; }
}