using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestProject.Shared.Persistence.Entities;

namespace TestProject.Shared.Persistence.Configuration;

public sealed class UserConfiguration : IEntityTypeConfiguration<UserRow>
{
    public void Configure(EntityTypeBuilder<UserRow> builder)
    {
        builder.ToTable("user");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        // Колонка "password" как в ТЗ, но храним хэш
        builder.Property(x => x.PasswordHash)
            .HasColumnName("password")
            .HasMaxLength(500)
            .IsRequired();
    }
}