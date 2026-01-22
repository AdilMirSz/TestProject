using Microsoft.EntityFrameworkCore;
using TestProject.Shared.Persistence;
using TestProject.UserService.Application.Abstractions;

namespace TestProject.UserService.Infrastructure.Persistence;

public sealed class EfUserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public EfUserRepository(AppDbContext db) => _db = db;

    public Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
        => _db.Users.AnyAsync(x => x.Name == name, ct);

    public Task<UserAuthData?> GetByNameAsync(string name, CancellationToken ct)
        => _db.Users
            .Where(x => x.Name == name)
            .Select(x => new UserAuthData(x.Id, x.Name, x.PasswordHash))
            .FirstOrDefaultAsync(ct);

    public async Task<long> CreateAsync(string name, string passwordHash, CancellationToken ct)
    {
        var row = new Shared.Persistence.Entities.UserRow
        {
            Name = name,
            PasswordHash = passwordHash
        };

        _db.Users.Add(row);
        await _db.SaveChangesAsync(ct);
        return row.Id;
    }
}