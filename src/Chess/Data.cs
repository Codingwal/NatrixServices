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

public class GameData(GameId gameId, bool isPublic, int timePerPlayer, string fen)
{
    [Key, Required, StringLength(8)]
    public GameId GameId { get; set; } = gameId;
    public bool IsPublic { get; set; } = isPublic;

    public string? Player1 { get; set; } = null;
    public string? Player2 { get; set; } = null;
    public int TimeLeft1 { get; set; } = timePerPlayer;
    public int TimeLeft2 { get; set; } = timePerPlayer;

    public string Fen { get; set; } = fen;
    public List<MoveDTO> Moves { get; set; } = [];

    public char? Result { get; set; } = null;
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