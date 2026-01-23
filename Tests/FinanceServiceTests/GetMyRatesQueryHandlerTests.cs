using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestProject.FinanceService.Application.Rates;
using TestProject.Shared.Persistence;
using TestProject.Shared.Persistence.Entities;
using Xunit;

public class GetMyRatesHandlerTests
{
    private readonly GetMyRatesHandler _handler;
    private readonly Mock<AppDbContext> _dbContextMock;

    public GetMyRatesHandlerTests()
    {
        _dbContextMock = new Mock<AppDbContext>();
        _handler = new GetMyRatesHandler(_dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCurrencyRates_WhenUserHasFavorites()
    {
        // Arrange
        var userId = 1;
        var query = new GetMyRatesQuery(userId);
        var userFavorites = new List<UserFavoriteCurrencyRow>
        {
            new UserFavoriteCurrencyRow { UserId = userId, CurrencyId = 1 },
            new UserFavoriteCurrencyRow { UserId = userId, CurrencyId = 2 }
        };
        var currencies = new List<CurrencyRow>
        {
            new CurrencyRow { Id = 1, Name = "USD", Rate = 75.0m, UpdatedAt = DateTime.UtcNow },
            new CurrencyRow { Id = 2, Name = "EUR", Rate = 85.0m, UpdatedAt = DateTime.UtcNow }
        };
        _dbContextMock.Setup(db => db.UserFavoriteCurrencies
                .Where(f => f.UserId == userId))
            .Returns(userFavorites.AsQueryable());
        _dbContextMock.Setup(db => db.Currencies)
            .Returns((DbSet<CurrencyRow>)currencies.AsQueryable());

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
}