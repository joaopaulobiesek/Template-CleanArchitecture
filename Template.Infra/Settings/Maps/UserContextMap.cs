namespace Template.Infra.Settings.Maps;

internal class UserContextMap : IEntityTypeConfiguration<ContextUser>
{
    public void Configure(EntityTypeBuilder<ContextUser> builder)
    {
        // Primary key
        builder.HasKey(u => u.Id);

        // Indexes for "normalized" username and email, to allow efficient lookups
        builder.HasIndex(u => u.NormalizedUserName).HasName("UserNameIndex").IsUnique();
        builder.HasIndex(u => u.NormalizedEmail).HasName("EmailIndex");

        // Maps to the AspNetUsers table
        builder.ToTable("AspNetUsers");

        // A concurrency token for use with the optimistic concurrency checking
        builder.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

        // Limit the size of columns to use efficient database types
        builder.Property(u => u.UserName).HasMaxLength(256);
        builder.Property(u => u.NormalizedUserName).HasMaxLength(256);
        builder.Property(u => u.Email).HasMaxLength(256);
        builder.Property(u => u.NormalizedEmail).HasMaxLength(256);
        builder.Property(u => u.ProfileImageUrl).HasMaxLength(1024);

        // The relationships between User and other entity types
        // Note that these relationships are configured with no navigation properties

        // Each User can have many UserClaims
        builder.HasMany<ContextUserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();

        // Each User can have many UserLogins
        builder.HasMany<ContextUserLogin>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();

        // Each User can have many UserTokens
        builder.HasMany<ContextUserToken>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

        // Each User can have many entries in the UserRole join table
        builder.HasMany<ContextUserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
    }
}

internal class ContextUserClaimMap : IEntityTypeConfiguration<ContextUserClaim>
{
    public void Configure(EntityTypeBuilder<ContextUserClaim> builder)
    {
        builder.HasKey(uc => uc.Id);

        // Maps to the AspNetUserClaims table
        builder.ToTable("AspNetUserClaims");
    }
}

internal class ContextUserLoginMap : IEntityTypeConfiguration<ContextUserLogin>
{
    public void Configure(EntityTypeBuilder<ContextUserLogin> builder)
    {
        builder.HasKey(l => new { l.LoginProvider, l.ProviderKey });

        // Limit the size of the composite key columns due to common DB restrictions
        builder.Property(l => l.LoginProvider).HasMaxLength(128);
        builder.Property(l => l.ProviderKey).HasMaxLength(128);

        // Maps to the AspNetUserLogins table
        builder.ToTable("AspNetUserLogins");
    }
}

internal class ContextUserTokenMap : IEntityTypeConfiguration<ContextUserToken>
{
    public void Configure(EntityTypeBuilder<ContextUserToken> builder)
    {
        builder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

        // Limit the size of the composite key columns due to common DB restrictions
        builder.Property(t => t.LoginProvider).HasMaxLength(1000);
        builder.Property(t => t.Name).HasMaxLength(1000);

        // Maps to the AspNetUserTokens table
        builder.ToTable("AspNetUserTokens");
    }
}

internal class ContextRoleMap : IEntityTypeConfiguration<ContextRole>
{
    public void Configure(EntityTypeBuilder<ContextRole> builder)
    {
        builder.HasKey(r => r.Id);

        // Index for "normalized" role name to allow efficient lookups
        builder.HasIndex(r => r.NormalizedName).HasName("RoleNameIndex").IsUnique();

        // Maps to the AspNetRoles table
        builder.ToTable("AspNetRoles");

        // A concurrency token for use with the optimistic concurrency checking
        builder.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

        // Limit the size of columns to use efficient database types
        builder.Property(u => u.Name).HasMaxLength(256);
        builder.Property(u => u.NormalizedName).HasMaxLength(256);

        // The relationships between Role and other entity types
        // Note that these relationships are configured with no navigation properties

        // Each Role can have many entries in the UserRole join table
        builder.HasMany<ContextUserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();

        // Each Role can have many associated RoleClaims
        builder.HasMany<ContextRoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
    }
}

internal class ContextUserRoleMap : IEntityTypeConfiguration<ContextUserRole>
{
    public void Configure(EntityTypeBuilder<ContextUserRole> builder)
    {
        builder.HasKey(r => new { r.UserId, r.RoleId });

        // Maps to the AspNetUserRoles table
        builder.ToTable("AspNetUserRoles");
    }
}

internal class ContextRoleClaimMap : IEntityTypeConfiguration<ContextRoleClaim>
{
    public void Configure(EntityTypeBuilder<ContextRoleClaim> builder)
    {
        // Primary key
        builder.HasKey(rc => rc.Id);

        // Maps to the AspNetRoleClaims table
        builder.ToTable("AspNetRoleClaims");
    }
}