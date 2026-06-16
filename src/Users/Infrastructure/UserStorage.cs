using Microsoft.EntityFrameworkCore;
using NatrixServices.Users.Application.Interfaces;

namespace NatrixServices.Users.Infrastructure;

public class UserStorage(DbContextOptions options) : DbContext(options), IUserStorage
{
    private DbSet<UserData> Users => Set<UserData>();
    public async Task InitAsync() => await Database.EnsureCreatedAsync();
    public async Task<IEnumerable<UserData>> GetAllUsersAsync()
    {
        return await Users.ToListAsync();
    }

    public async Task<UserData?> GetUserAsync(string username)
    {
        return await Users.FindAsync(username);
    }

    public async Task SaveUserAsync(UserData user)
    {
        Users.Update(user);
        await SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserData>(builder =>
        {
            builder.HasKey(u => u.Username);
        });
    }
}