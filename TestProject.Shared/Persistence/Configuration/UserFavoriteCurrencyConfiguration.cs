using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestProject.Shared.Persistence.Entities;

namespace TestProject.Shared.Persistence.Configuration;

public sealed class UserFavoriteCurrencyConfiguration : IEntityTypeConfiguration<UserFavoriteCurrencyRow>
{
    public void Configure(EntityTypeBuilder<UserFavoriteCurrencyRow> builder)
    {
        builder.ToTable("user_favorite_currency");

        builder.HasKey(x => new { x.UserId, x.CurrencyId });

        builder.Property(x => x.UserId)
            .HasColumnName("user_id");

        builder.Property(x => x.CurrencyId)
            .HasColumnName("currency_id");

        builder.HasOne(x => x.User)
            .WithMany(x => x.FavoriteCurrencies)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Currency)
            .WithMany(x => x.FavoritedByUsers)
            .HasForeignKey(x => x.CurrencyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.CurrencyId);
    }
}