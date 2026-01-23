using Moq;

namespace Tests.FinanceServiceTests;

public class GetMyRatesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsCurrencyRates()
    {
        // Arrange
        var mockCurrencyRepo = new Mock<ICurrencyRepository>();
        var userId = 1;
        var currencies = new List<CurrencyRateDto>
        {
            new CurrencyRateDto { Id = 1, Name = "USD", Rate = 75.00M },
            new CurrencyRateDto { Id = 2, Name = "EUR", Rate = 85.00M }
        };
        mockCurrencyRepo.Setup(repo => repo.GetRatesByUserIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(currencies);

        var handler = new GetMyRatesQueryHandler(mockCurrencyRepo.Object);

        var query = new GetMyRatesQuery(userId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("USD", result[0].Name);
        Assert.Equal(75.00M, result[0].Rate);
    }
}
