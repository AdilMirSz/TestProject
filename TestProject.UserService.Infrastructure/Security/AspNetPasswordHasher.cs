using Microsoft.AspNetCore.Identity;
using TestProject.UserService.Application.Abstractions;

namespace TestProject.UserService.Infrastructure.Security;

public sealed class AspNetPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<string> _hasher = new();

    public string Hash(string password)
        => _hasher.HashPassword("user", password);

    public bool Verify(string password, string passwordHash)
        => _hasher.VerifyHashedPassword("user", passwordHash, password) == PasswordVerificationResult.Success;
}