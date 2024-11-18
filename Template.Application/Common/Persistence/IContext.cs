using Template.Domain.Entity;

namespace Template.Application.Common.Persistence;

public interface IContext
{
    DbSet<Client> Clients { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}