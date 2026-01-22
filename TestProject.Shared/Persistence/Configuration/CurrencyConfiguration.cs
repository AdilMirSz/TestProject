using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestProject.Shared.Persistence.Entities;

namespace TestProject.Shared.Persistence.Configuration;

public sealed class CurrencyConfiguration : IEntityTypeConfiguration<CurrencyRow>
{
    public void Configure(EntityTypeBuilder<CurrencyRow> builder)
    {
        builder.ToTable("currency");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Rate)
            .HasColumnName("rate")
            .HasColumnType("numeric(18,6)")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
    }
}