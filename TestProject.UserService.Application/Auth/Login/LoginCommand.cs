using MediatR;

namespace TestProject.UserService.Application.Auth.Login;

public sealed record LoginCommand(string Name, string Password) : IRequest<LoginResult>;
public sealed record LoginResult(string AccessToken);