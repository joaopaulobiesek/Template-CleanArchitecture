using Template.Domain.Entity;
using Template.Infra.Persistence.Contexts;

namespace Template.Infra.Settings.Maps;

internal class ClientMap : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable(nameof(Client), Schema.Default);

        builder.HasKey(u => u.Id);
        builder.Property(u => u.FullName).IsRequired(true).HasMaxLength(100);
        builder.Property(u => u.DocumentNumber).IsRequired(true).HasMaxLength(20);
        builder.Property(u => u.ZipCode).IsRequired(true).HasMaxLength(10);
        builder.Property(u => u.Phone).IsRequired(false).HasMaxLength(20);
        builder.Property(u => u.Email).IsRequired(false).HasMaxLength(254);
    }
}