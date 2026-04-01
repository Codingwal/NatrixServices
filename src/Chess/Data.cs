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
    public int GamesLost { get; set; } = 0;
    public int GamesDrawn => GamesPlayed - (GamesWon + GamesLost);
    public double WinRate => Average(GamesWon);
    public double LostRate => Average(GamesLost);
    public double DrawRate => Average(GamesDrawn);

    public double AverageTimeLeftPercent { get; set; } = 0;

    public Dictionary<char, int> PiecesTaken { get; set; } = new() { { 'p', 0 }, { 'r', 0 }, { 'n', 0 }, { 'b', 0 }, { 'q', 0 } };
    public int PiecesTakenTotal => PiecesTaken.Values.Sum();
    public double PiecesTakenAverage => Average(PiecesTakenTotal);

    public Dictionary<char, int> PiecesLost { get; set; } = new() { { 'p', 0 }, { 'r', 0 }, { 'n', 0 }, { 'b', 0 }, { 'q', 0 } };
    public int PiecesLostTotal => PiecesLost.Values.Sum();
    public double PiecesLostAverage => Average(PiecesLostTotal);

    public int TotalMoves { get; set; } = 0;
    public double AverageMoves => Average(TotalMoves);

    public (string, string) GetTitle()
    {
        var titles = GetTitles();
        if (titles.Count == 0) return ("", "");

        Random rnd = new(DateTimeOffset.UtcNow.Minute); // Must not change too fast as the function is called twice very quickly and should give the same result
        int index = (int)rnd.NextInt64(0, titles.Count - 1);
        return titles[index];
    }
    public List<(string, string)> GetTitles()
    {
        List<(string, string)> titles = [];

        if (WinRate >= 0.9) titles.Add(("Perfectionist", "Win over 90% of all games"));
        if (GamesPlayed >= 100) titles.Add(("Veteran", "Play over 100 games"));
        if (AverageTimeLeftPercent <= 0.2) titles.Add(("Just on time", "Finish games with under 20% of time left (on average)"));
        if (AverageTimeLeftPercent >= 0.7) titles.Add(("The flash", "Finish with over 70% of your time left on average"));
        if (PiecesTakenAverage >= 10) titles.Add(("Destroyer", "Destroy more than 10 pieces on average"));
        if (PiecesLostAverage <= 8) titles.Add(("Gentleman", "Lose under 8 pieces on average"));
        if (AverageMoves >= 40) titles.Add(("Tactician", "Make over 40 moves per game"));
        if (DrawRate >= 0.4) titles.Add(("Businessman", "End over 40% of games in a draw"));
        if (PiecesLostAverage >= 12) titles.Add(("Suicidal", "Lose over 12 pieces per game (on average)"));
        if (Average(PiecesLost['p']) <= 4) titles.Add(("Man of the people", "Have at least half your pawns left on average"));
        if (Average(PiecesLost['n']) <= 1) titles.Add(("Horsekeeper", "Keep at least one horse alive on average"));
        if (Average(PiecesLost['q']) <= 0.3) titles.Add(("Girlpower", "Lose your queen in under 30% of games"));
        if (Average(PiecesLost['r']) >= 1.5) titles.Add(("Fortress", "Finish the game with one and a half rooks left on average"));
        if (Average(PiecesTaken['r']) >= 1.2) titles.Add(("Bombardier", "Destroy at least 1.2 rooks on average"));
        if (Average(PiecesTaken['b']) >= 1) titles.Add(("The wall", "Capture at least one bishop per game (on average)"));

        return titles;
    }
    private double Average(int totalValue)
    {
        return GamesPlayed != 0 ? (double)totalValue / GamesPlayed : 0;
    }

    public void UpdateStats(GameData gameData, Players player)
    {
        ChessGame game = new(gameData.Fen);

        // Update GamesPlayed, GamesWon, GamesLost
        GamesPlayed++;
        if (gameData.Result == 'w')
        {
            if (player == Players.White) GamesWon++;
            else GamesLost++;
        }
        else if (gameData.Result == 'b')
        {
            if (player == Players.Black) GamesWon++;
            else GamesLost++;
        }

        // Update TimeLeftPercent
        TimeSpan timeLeft = (player == Players.White) ? gameData.TimeLeft1 : gameData.TimeLeft2;
        double timeLeftPercent = timeLeft / gameData.TimePerPlayer;
        AverageTimeLeftPercent = (AverageTimeLeftPercent * (GamesPlayed - 1) + timeLeftPercent) / GamesPlayed;

        // Update FiguresLost and FiguresTaken
        foreach (char piece in "rnbqp")
        {
            int countOwn = game.CountPieces(piece, player);
            int countOpponent = game.CountPieces(piece, ChessEngine.OtherPlayer(player));

            int startCount = piece switch
            {
                'r' or 'n' or 'b' => 2,
                'q' => 1,
                'p' => 8,
                _ => throw new("Invalid piece")
            };

            PiecesLost[piece] += startCount - countOwn;
            PiecesTaken[piece] += startCount - countOpponent;
        }

        TotalMoves += gameData.Moves.Count / 2;
    }
}

public class GameData
{
    [Key, Required, StringLength(8)]
    public GameId GameId { get; set; } = GameId.Empty;
    public bool IsPublic { get; set; } = false;

    public string? Player1 { get; set; } = null;
    public string? Player2 { get; set; } = null;

    public TimeSpan TimePerPlayer { get; set; }
    public TimeSpan TimeLeft1 { get; set; }
    public TimeSpan TimeLeft2 { get; set; }
    public DateTimeOffset LastMoveTime { get; set; } = default;

    public string Fen { get; set; } = string.Empty;
    public List<MoveDTO> Moves { get; set; } = [];

    public char? Result { get; set; } = null; // optional. 'w' => white won, 'b' => black won, 'd' => draw

    public GameData() { }
    public GameData(GameId gameId, bool isPublic, int timePerPlayer, string fen)
    {
        GameId = gameId;
        IsPublic = isPublic;
        TimePerPlayer = TimeSpan.FromMinutes(timePerPlayer);
        TimeLeft1 = TimePerPlayer;
        TimeLeft2 = TimePerPlayer;
        Fen = fen;
    }
}

public record ChessBoardDTO
{
    public char[][] Board { get; set; } = new char[8][];

    public ChessBoardDTO() { }
    public ChessBoardDTO(ChessGame game)
    {
        for (int y = 0; y < 8; y++)
        {
            Board[y] = new char[8];
            for (int x = 0; x < 8; x++)
            {
                Board[y][x] = game.Fields[x, y];
            }
        }
    }
}

public record UserDataDTO(UserData userData)
{
    public string Username { get; set; } = userData.Username;
    public string Title { get; set; } = userData.TitleHolder ? "Title holder" : userData.Stats.GetTitle().Item1;
    public string TitleDescription { get; set; } = userData.TitleHolder ? "The reigning champion" : userData.Stats.GetTitle().Item2;
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