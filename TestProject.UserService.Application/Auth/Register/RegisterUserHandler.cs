using MediatR;
using TestProject.UserService.Application.Abstractions;

namespace TestProject.UserService.Application.Auth.Register;

public sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;

    public RegisterUserHandler(IUserRepository users, IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Password is required.");

        if (await _users.ExistsByNameAsync(name, ct))
            throw new InvalidOperationException("User already exists.");

        var hash = _hasher.Hash(request.Password);
        var userId = await _users.CreateAsync(name, hash, ct);

        return new RegisterUserResult(userId);
    }
}