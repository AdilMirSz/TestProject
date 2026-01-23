using MediatR;

namespace TestProject.UserService.Application.Currency;

public sealed record RemoveCurrencyFromFavoritesCommand(long UserId, long CurrencyId) : IRequest;