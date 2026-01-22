using MediatR;

namespace TestProject.UserService.Application.Auth.Register;

public sealed record RegisterUserCommand(string Name, string Password) : IRequest<RegisterUserResult>;
public sealed record RegisterUserResult(long UserId);