using MediatR;

namespace TestProject.UserService.Application.Currency;

public sealed record AddCurrencyToFavoritesCommand(long UserId, long CurrencyId) : IRequest;