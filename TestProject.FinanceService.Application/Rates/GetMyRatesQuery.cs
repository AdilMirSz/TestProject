using MediatR;

namespace TestProject.FinanceService.Application.Rates;

public sealed record GetMyRatesQuery(long UserId) : IRequest<IReadOnlyList<CurrencyRateDto>>;