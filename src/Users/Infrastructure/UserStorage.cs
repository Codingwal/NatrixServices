using Microsoft.EntityFrameworkCore;
using NatrixServices.Shared.Infrastructure.Database;
using NatrixServices.Users.Application.Interfaces;
using NatrixServices.Users.Core.Entities;

namespace NatrixServices.Users.Infrastructure;

public class UserStorage(DbContextOptions<UserStorage> options) : DbContext(options), IUserStorage
{
    private DbSet<UserData> Users => Set<UserData>();
    public async Task InitAsync() => await Database.EnsureCreatedAsync();
    public async Task<IEnumerable<UserData>> GetAllUsersAsync()
        => await Users.ToListAsync();

    public async Task<UserData?> GetUserAsync(string username)
        => await Users.FindAsync(username);

    public Task AddUserAsync(UserData user)
        => ItemStorageUtility.AddEntityAsync(this, user, user.Username);

    public Task UpdateUserAsync(UserData user)
        => ItemStorageUtility.UpdateEntityAsync(this, user, u => u.Username);

    public async Task<bool> UserExists(string username)
        => await Users.AnyAsync(u => u.Username == username);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserData>(builder =>
        {
            builder.HasKey(u => u.Username);
        });
    }
}