using Template.Domain.Entity;

namespace Template.Infra.Persistence.Contexts;

public class Context : IdentityDbContext<ContextUser, ContextRole, string, ContextUserClaim, ContextUserRole, ContextUserLogin, ContextRoleClaim, ContextUserToken>, IContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInformation()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Entity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (Entity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.Updated();
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.Updated();

                var activeProperty = entry.Property(nameof(Entity.Active));
                if (activeProperty.IsModified && !entity.Active)
                {
                    entity.Delete();
                }
            }
        }
    }
}