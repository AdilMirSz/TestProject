using MediatR;
using Microsoft.EntityFrameworkCore;
using TestProject.Shared.Persistence;

namespace TestProject.UserService.Application.Currency;

public sealed class RemoveCurrencyFromFavoritesHandler : IRequestHandler<RemoveCurrencyFromFavoritesCommand>
{
    private readonly AppDbContext _context;

    public RemoveCurrencyFromFavoritesHandler(AppDbContext context) => _context = context;

    public async Task Handle(RemoveCurrencyFromFavoritesCommand request, CancellationToken ct)
    {
        var row = await _context.UserFavoriteCurrencies
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.CurrencyId == request.CurrencyId, ct);

        if (row is null) return;

        _context.UserFavoriteCurrencies.Remove(row);
        await _context.SaveChangesAsync(ct);
    }
}