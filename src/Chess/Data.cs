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

        modelBuilder.Entity<UserData>(b =>
        {
            b.OwnsOne(u => u.Stats);
        });
        
        modelBuilder.Entity<GameData>(b =>
        {
            b.Property(g => g.Moves).SaveAsJson();
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
    public bool TitleHolder { get; set; } = false;
    public int SeasonsWon { get; set; } = 0;
    public int TournamentsWon { get; set; } = 0;
    public UserStats Stats { get; set; } = new();
}
public class GlobalData
{

}

public class UserStats
{
    public int GamesPlayed { get; set; } = 0;
    public int GamesWon { get; set; } = 0;
    public float WinRate => GamesPlayed != 0 ? (float)GamesWon / GamesPlayed : 0;

    public string GetTitle()
    {
        throw new NotImplementedException();
    }
}

public class GameData
{
    [Key, Required, StringLength(8)]
    public GameId GameId { get; set; } = GameId.Empty;
    public bool IsPublic { get; set; } = false;

    public string? Player1 { get; set; } = null;
    public string? Player2 { get; set; } = null;
    public int TimeLeft1 { get; set; } = -1;
    public int TimeLeft2 { get; set; } = -1;

    public string Fen { get; set; } = string.Empty;
    public List<MoveDTO> Moves { get; set; } = [];

    public char? Result { get; set; } = null;

    public GameData() { }
    public GameData(GameId gameId, bool isPublic, int timePerPlayer, string fen)
    {
        GameId = gameId;
        IsPublic = isPublic;
        TimeLeft1 = timePerPlayer;
        TimeLeft2 = timePerPlayer;
        Fen = fen;
    }
}

public record ChessBoardDTO
{
    public char[,] Board { get; set; } = new char[8, 8];
}

public record UserDataDTO(UserData userData)
{
    public string Username { get; set; } = userData.Username;
    public string Title { get; set; } = userData.TitleHolder ? "Title holder" : userData.Stats.GetTitle();
    public int SeasonsWon { get; set; } = userData.SeasonsWon;
    public int TournamentsWon { get; set; } = userData.TournamentsWon;
}

public record MoveDTO
{
    public string From { get; set; }
    public string To { get; set; }
    public char? Promotion { get; set; }

    public MoveDTO(Move move)
    {
        From = ChessEngine.PosToFieldDesc(move.Origin);
        To = ChessEngine.PosToFieldDesc(move.Destination);
        Promotion = move.Promotion;
    }
}

public record GameListDTO(List<GameData> Games);
public record MoveListDTO(List<MoveDTO> Moves);