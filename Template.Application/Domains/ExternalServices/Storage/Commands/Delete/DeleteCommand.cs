using Template.Application.Common.Security;
using Template.Domain.Constants;

namespace Template.Application.Domains.ExternalServices.Storage.Commands.Delete;

[Authorize(Roles = Roles.Admin)]
[Authorize(Policy = Policies.CanPurge)]
public class FileDeleteCommand
{
    public string FileName { get; set; }
}