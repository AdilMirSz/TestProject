using MediatR;
using Microsoft.EntityFrameworkCore;
using TestProject.Shared.Persistence;

namespace TestProject.UserService.Application.Currency;

public sealed class GetMyFavoritesHandler : IRequestHandler<GetMyFavoritesQuery, IReadOnlyList<FavoriteCurrencyDto>>
{
    private readonly AppDbContext _context;

    public GetMyFavoritesHandler(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<FavoriteCurrencyDto>> Handle(GetMyFavoritesQuery request, CancellationToken ct)
    {
        return await _context.UserFavoriteCurrencies
            .Where(f => f.UserId == request.UserId)
            .Join(_context.Currencies,
                f => f.CurrencyId,
                c => c.Id,
                (f, c) => new { c.Id, c.Name })
            .OrderBy(x => x.Name) 
            .Select(x => new FavoriteCurrencyDto(x.Id, x.Name))
            .ToListAsync(ct);
    }
}