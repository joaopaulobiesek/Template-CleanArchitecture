using Template.Domain.Entity.Tenant;

namespace Template.Application.Common.Persistence;

public interface ITenantContext
{
    DbSet<Client> Clients { get; }
    DbSet<ClientModule> ClientModules { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}