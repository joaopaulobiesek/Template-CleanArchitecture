using Template.Application.Common.Security;
using Template.Domain.Constants;
using Template.Domain.Interfaces;

namespace Template.Application.Domains.InternalServices.Clients.Commands.UpdateClient;

[Authorize(Roles = Roles.Admin)]
[Authorize(Policy = Policies.CanEdit)]
public class UpdateClientCommand : IUpdateClient
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public string? ZipCode { get; set; }
}