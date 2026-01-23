using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestProject.FinanceService.Application.Rates;
using TestProject.Shared.Persistence;
using TestProject.UserService.Application.Abstractions;
using TestProject.UserService.Infrastructure.Persistence;

namespace Tests.UserServiceTests;

public class UserFinanceIntegrationTests
{
    private readonly IServiceProvider _serviceProvider;

    public UserFinanceIntegrationTests()
    {
        // Setup in-memory database or actual database connection here
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));
        serviceCollection.AddTransient<IUserRepository, EfUserRepository>();
        serviceCollection.AddTransient<GetMyRatesHandler>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task UserShouldGetFavoriteCurrencyRates()
    {
        // Arrange
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var financeHandler = _serviceProvider.GetRequiredService<GetMyRatesHandler>();

        // Create user
        var userId = await userRepository.CreateAsync("testuser", "hashedPassword", default);

        // Simulate adding user favorites and fetching rates
        var query = new GetMyRatesQuery(userId);
        var result = await financeHandler.Handle(query, default);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal("USD", result[0].Name);
    }
}