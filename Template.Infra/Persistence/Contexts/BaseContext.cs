using Template.Infra.Settings.Maps;

namespace Template.Infra.Persistence.Contexts;

public class BaseContext : IdentityDbContext<ContextUser, ContextRole, string, ContextUserClaim, ContextUserRole, ContextUserLogin, ContextRoleClaim, ContextUserToken>
{
    public BaseContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserBaseContextMap());
        modelBuilder.ApplyConfiguration(new ContextUserClaimMap());
        modelBuilder.ApplyConfiguration(new ContextUserLoginMap());
        modelBuilder.ApplyConfiguration(new ContextUserTokenMap());
        modelBuilder.ApplyConfiguration(new ContextRoleMap());
        modelBuilder.ApplyConfiguration(new ContextUserRoleMap());
        modelBuilder.ApplyConfiguration(new ContextRoleClaimMap());
    }
}