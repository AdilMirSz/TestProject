namespace TestProject.FinanceService.Application.Rates;

public sealed record CurrencyRateDto(long CurrencyId, string Name, decimal Rate, DateTime UpdatedAt);