namespace TestProject.Shared.Persistence.Entities;

public sealed class CurrencyRow
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Rate { get; set; }

    // Не в ТЗ, но очень полезно для понимания актуальности данных
    public DateTime UpdatedAt { get; set; }

    public ICollection<UserFavoriteCurrencyRow> FavoritedByUsers { get; set; } = new List<UserFavoriteCurrencyRow>();
}