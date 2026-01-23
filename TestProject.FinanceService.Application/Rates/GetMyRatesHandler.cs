using MediatR;
using Microsoft.EntityFrameworkCore;
using TestProject.Shared;
using TestProject.Shared.Persistence;

namespace TestProject.FinanceService.Application.Rates;

public sealed class GetMyRatesHandler : IRequestHandler<GetMyRatesQuery, IReadOnlyList<CurrencyRateDto>>
{
    private readonly AppDbContext _db;

    public GetMyRatesHandler(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<CurrencyRateDto>> Handle(GetMyRatesQuery request, CancellationToken ct)
    {
        // join favorites -> currency, отдаём name/rate/updatedAt
        return await _db.UserFavoriteCurrencies
            .Where(f => f.UserId == request.UserId)
            .Join(_db.Currencies,
                f => f.CurrencyId,
                c => c.Id,
                (f, c) => new { c.Id, c.Name, c.Rate, c.UpdatedAt })
            .OrderBy(x => x.Name)
            .Select(x => new CurrencyRateDto(x.Id, x.Name, x.Rate, x.UpdatedAt))
            .ToListAsync(ct);
    }
}