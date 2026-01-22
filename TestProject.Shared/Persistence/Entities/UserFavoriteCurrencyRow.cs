namespace TestProject.Shared.Persistence.Entities;

public sealed class UserFavoriteCurrencyRow
{
    public long UserId { get; set; }
    public UserRow User { get; set; } = null!;

    public long CurrencyId { get; set; }
    public CurrencyRow Currency { get; set; } = null!;
}