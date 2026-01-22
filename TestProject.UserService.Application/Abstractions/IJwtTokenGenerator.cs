namespace TestProject.UserService.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(long userId, string name);
}