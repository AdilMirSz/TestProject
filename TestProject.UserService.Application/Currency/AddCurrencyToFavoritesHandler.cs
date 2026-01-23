using MediatR;
using Microsoft.EntityFrameworkCore;
using TestProject.Shared.Persistence;
using TestProject.Shared.Persistence.Entities;

namespace TestProject.UserService.Application.Currency;

public sealed class AddCurrencyToFavoritesHandler : IRequestHandler<AddCurrencyToFavoritesCommand>
{
    private readonly AppDbContext _context;

    public AddCurrencyToFavoritesHandler(AppDbContext context) => _context = context;

    public async Task Handle(AddCurrencyToFavoritesCommand request, CancellationToken ct)
    {
        // 1) Проверка что user существует (опционально, но хорошо)
        var userExists = await _context.Users.AnyAsync(x => x.Id == request.UserId, ct);
        if (!userExists) throw new InvalidOperationException("User not found.");

        // 2) Проверка что currency существует
        var currencyExists = await _context.Currencies.AnyAsync(x => x.Id == request.CurrencyId, ct);
        if (!currencyExists) throw new InvalidOperationException("Currency not found.");

        // 3) Если уже есть — просто выходим (идемпотентно)
        var exists = await _context.UserFavoriteCurrencies
            .AnyAsync(x => x.UserId == request.UserId && x.CurrencyId == request.CurrencyId, ct);

        if (exists) return;

        _context.UserFavoriteCurrencies.Add(new UserFavoriteCurrencyRow
        {
            UserId = request.UserId,
            CurrencyId = request.CurrencyId
        });

        await _context.SaveChangesAsync(ct);
    }
}