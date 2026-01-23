using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TestProject.UserService.Application.Auth.Register;

namespace Tests.IntegrationTests;

public class UserControllerTests : IClassFixture<WebApplicationFactory<TestProject.UserService.WebApi.Startup>>
{
    private readonly HttpClient _client;

    public UserControllerTests(WebApplicationFactory<TestProject.UserService.WebApi.Startup> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ReturnsCreatedAtAction()
    {
        var registerUser = new RegisterUserCommand { Name = "alice", Password = "secret" };

        var response = await _client.PostAsJsonAsync("/api/user/register", registerUser);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("userId", responseContent);
    }
}
