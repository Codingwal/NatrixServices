using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices.Chess;

public class DataContext(DbContextOptions<DataContext> options) : DataContext<UserData, GlobalData>(options)
{
    public DbSet<GameData> GameData => Set<GameData>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GameData>().ToTable("GameData");

        modelBuilder.Entity<GameData>(b =>
        {
            b.Property(u => u.Game).SaveAsJson();
        });

        modelBuilder.Entity<UserData>(b =>
        {
            b.Property(u => u.Games).SaveAsJson();
        });
    }

    public async Task<GameData?> GetGameData(GameId gameId)
    {
        GameData? gameData = await GameData.FindAsync(gameId);
        return gameData;
    }
}


public class UserData : UserDataBase
{
    public List<GameId> Games { get; set; } = [];
}
public class GlobalData
{

}
public class GameData
{
    [Key, Required, StringLength(8)]
    public GameId Id { get; set; } = GameId.Empty;
    public bool IsPublic { get; set; } = false;
    public UserId? Player1 { get; set; } = null;
    public UserId? Player2 { get; set; } = null;
    public ChessGame Game { get; set; } = new();
}