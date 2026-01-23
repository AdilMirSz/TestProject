using MediatR;

namespace TestProject.UserService.Application.Currency;

public sealed record GetMyFavoritesQuery(long UserId) : IRequest<IReadOnlyList<FavoriteCurrencyDto>>;