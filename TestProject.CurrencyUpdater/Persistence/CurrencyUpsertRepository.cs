namespace TestProject.CurrencyUpdater.Persistence;

using Microsoft.EntityFrameworkCore;
using Shared.Persistence;

public sealed class CurrencyUpsertRepository
{
    private readonly AppDbContext _db;

    public CurrencyUpsertRepository(AppDbContext db) => _db = db;

    public async Task UpsertManyAsync(
        IReadOnlyCollection<(string Name, decimal Rate)> items,
        DateTime updatedAtUtc,
        CancellationToken ct)
    {
        if (items.Count == 0) return;

        
        foreach (var (name, rate) in items)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync($@"
INSERT INTO currency (name, rate, updated_at)
VALUES ({name}, {rate}, {updatedAtUtc})
ON CONFLICT (name)
DO UPDATE SET
    rate = EXCLUDED.rate,
    updated_at = EXCLUDED.updated_at;
", ct);
        }
    }
}
