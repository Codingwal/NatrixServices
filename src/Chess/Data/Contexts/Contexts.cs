using Microsoft.EntityFrameworkCore;

namespace NatrixServices.Chess.Data;

public class GameDataContext(DbContextOptions<GameDataContext> options) : DbContext(options)
{
    public DbSet<GameData> GameData => Set<GameData>();

    public void Init() => Database.EnsureCreated();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameData>(b =>
        {
            b.Property(g => g.Moves).SaveAsJson();
        });
    }

    public async Task<GameData?> GetGameDataAsync(GameId gameId)
    {
        GameData? gameData = await GameData.FindAsync(gameId);
        return gameData;
    }
    public async Task<List<GameData>> GetAllGamesAsync()
    {
        return await GameData.ToListAsync();
    }
    public async Task AddGameDataAsync(GameData gameData)
    {
        await GameData.AddAsync(gameData);
        await SaveChangesAsync();
    }
}

public class UserDataContext(DbContextOptions<UserDataContext> options) : DbContext(options)
{
    public DbSet<UserData> UserData => Set<UserData>();

    public void Init() => Database.EnsureCreated();
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