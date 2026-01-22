namespace TestProject.UserService.Application.Abstractions;

public interface IUserRepository
{
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
    Task<UserAuthData?> GetByNameAsync(string name, CancellationToken ct);
    Task<long> CreateAsync(string name, string passwordHash, CancellationToken ct);
}

public sealed record UserAuthData(long Id, string Name, string PasswordHash);