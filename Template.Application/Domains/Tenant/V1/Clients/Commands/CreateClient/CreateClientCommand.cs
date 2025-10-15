using Template.Application.Common.Security;
using Template.Domain.Constants;
using Template.Domain.Interfaces.Tenant;

namespace Template.Application.Domains.Tenant.V1.Clients.Commands.CreateClient;

[Authorize(Roles = Roles.Admin)]
[Authorize(Policy = Policies.CanCreate)]
public class CreateClientCommand : ICreateClient
{
    public string FullName { get; set; }
    public string DocumentNumber { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public string? ZipCode { get; set; }
    public bool Paid { get; set; }
}