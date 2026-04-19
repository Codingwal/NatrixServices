using Microsoft.EntityFrameworkCore;

namespace NatrixServices.Chess;

public class UserData : UserDataBase
{
    public bool TitleHolder { get; set; } = false;
    public int SeasonsWon { get; set; } = 0;
    public int TournamentsWon { get; set; } = 0;
    public UserStatsDTO Stats { get; set; } = new();
}

public class UserDataContext(DbContextOptions<UserDataContext> options) : DbContext(options)
{
    public DbSet<UserData> UserData => Set<UserData>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserData>(b =>
        {
            b.Property(u => u.Stats).SaveAsJson();
        });
    }

    public async Task<UserData> GetOrCreateUserAsync(string username)
    {
        UserData? userData = UserData.Find(username);
        if (userData == null)
        {
            userData = new UserData { Username = username };
            UserData.Add(userData);
            await SaveChangesAsync();
        }
        return userData;
    }
}