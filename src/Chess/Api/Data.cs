using Microsoft.EntityFrameworkCore;

namespace NatrixServices.Chess;

public class DataContext(DbContextOptions<DataContext> options) : DataContext<UserData, GlobalData>(options)
{
    public DbSet<GameDataDTO> GameData => Set<GameDataDTO>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GameDataDTO>().ToTable("GameData");

        modelBuilder.Entity<UserData>(b =>
        {
            b.OwnsOne(u => u.Stats);
        });

        modelBuilder.Entity<GameDataDTO>(b =>
        {
            b.Property(g => g.Moves).SaveAsJson();
        });
    }

    public async Task<GameDataDTO?> GetGameData(GameId gameId)
    {
        GameDataDTO? gameData = await GameData.FindAsync(gameId);
        return gameData;
    }
}


public class UserData : UserDataBase
{
    public bool TitleHolder { get; set; } = false;
    public int SeasonsWon { get; set; } = 0;
    public int TournamentsWon { get; set; } = 0;
    public UserStatsDTO Stats { get; set; } = new();
}
public class GlobalData
{

}