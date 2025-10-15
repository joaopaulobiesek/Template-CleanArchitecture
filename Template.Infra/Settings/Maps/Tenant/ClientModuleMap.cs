using Template.Infra.Persistence.Contexts;

namespace Template.Infra.Settings.Maps.Tenant;

internal class ClientModuleMap : IEntityTypeConfiguration<ClientModule>
{
    public void Configure(EntityTypeBuilder<ClientModule> builder)
    {
        builder.ToTable(nameof(ClientModule), Schema.Default);

        builder.HasKey(cm => cm.Id);

        builder.Property(cm => cm.Module)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cm => cm.ActivatedAt)
            .IsRequired();

        builder.Property(cm => cm.DeactivatedAt)
            .IsRequired(false);

        builder.Property(cm => cm.ClientId)
            .IsRequired();

        builder.HasOne(cm => cm.Client)
            .WithMany() 
            .HasForeignKey(cm => cm.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
