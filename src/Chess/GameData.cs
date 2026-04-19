using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices.Chess;

public class GameData
{
    [Key, Required, StringLength(8)]
    public GameId GameId { get; set; } = GameId.Empty;
    public string Name { get; set; } = string.Empty;

    public bool IsPublic { get; set; } = false;

    public string? Player1 { get; set; } = null;
    public string? Player2 { get; set; } = null;

    public TimeSpan TimePerPlayer { get; set; }
    public TimeSpan TimeLeft1 { get; set; }
    public TimeSpan TimeLeft2 { get; set; }
    public DateTimeOffset LastMoveTime { get; set; } = default;

    public string Fen { get; set; } = string.Empty;
    public List<Move> Moves { get; set; } = [];

    public char? Result { get; set; } = null; // optional. 'w' => white won, 'b' => black won, 'd' => draw

    public GameData() { }
    public GameData(GameId gameId, string name, bool isPublic, int timePerPlayer, string fen)
    {
        GameId = gameId;
        Name = name;
        IsPublic = isPublic;
        TimePerPlayer = TimeSpan.FromMinutes(timePerPlayer);
        TimeLeft1 = TimePerPlayer;
        TimeLeft2 = TimePerPlayer;
        Fen = fen;
    }
}

public class GameDataContext(DbContextOptions<GameDataContext> options) : DbContext(options)
{
    public DbSet<GameData> GameData => Set<GameData>();

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