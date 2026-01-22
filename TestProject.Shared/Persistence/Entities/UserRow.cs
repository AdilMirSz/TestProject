namespace TestProject.Shared.Persistence.Entities;

public sealed class UserRow
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;

    // В БД колонку назовём "password" как в ТЗ, но в коде пусть будет PasswordHash
    public string PasswordHash { get; set; } = null!;

    public ICollection<UserFavoriteCurrencyRow> FavoriteCurrencies { get; set; } = new List<UserFavoriteCurrencyRow>();
}