using Moq;
using TestProject.Shared.Persistence.Entities;
using TestProject.UserService.Application.Abstractions;
using TestProject.UserService.Application.Auth.Login;

namespace Tests.UserServiceTests;

public class LoginHandlerTests
{
    [Fact]
    public async Task Handle_ValidLogin_ReturnsToken()
    {
        // Arrange
        var mockUserRepo = new Mock<IUserRepository>();
        var mockJwtGenerator = new Mock<IJwtTokenGenerator>();
        var mockHasher = new Mock<IPasswordHasher>();

        var user = new UserRow { Id = 1, Name = "alice", PasswordHash = "hashedpassword" };
        mockUserRepo.Setup(repo => repo.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockHasher.Setup(hasher => hasher.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        mockJwtGenerator.Setup(generator => generator.GenerateAccessToken(It.IsAny<long>(), It.IsAny<string>()))
            .Returns("generated-jwt-token");

        var handler = new LoginHandler(mockUserRepo.Object, mockHasher.Object, mockJwtGenerator.Object);
        
        var command = new LoginCommand { Name = "alice", Password = "secret" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("generated-jwt-token", result.Token);
    }
}
