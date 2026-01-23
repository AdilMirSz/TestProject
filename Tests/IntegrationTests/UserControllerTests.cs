using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using TestProject.UserService.Application.Auth.Register;
using Xunit;

namespace Tests.IntegrationTests
{
    public class UserControllerTests : IClassFixture<WebApplicationFactory<TestProject.UserService.WebApi.Program>>
    {
        private readonly HttpClient _client;

        public UserControllerTests(WebApplicationFactory<TestProject.UserService.WebApi.Program> factory)
        {
            _client = factory.CreateClient();
        }


        [Fact]
        public async Task Register_ReturnsCreatedAtAction()
        {
            // Arrange
            var registerUser = new RegisterUserCommand("alice", "secret");

            // Act
            var response = await _client.PostAsJsonAsync("/api/user/register", registerUser);

            // Assert
            response.EnsureSuccessStatusCode(); // Убедитесь, что статус 201 Created
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("userId", responseContent); // Проверяем, что userId есть в ответе
        }
    }
}