using System.ComponentModel.DataAnnotations;
using NatrixServices.Chess.Core;
using NatrixServices.Chess.Data;

namespace NatrixServices.Chess.API;

public record GameDataDTO(
    GameId GameId,
    string Name,
    bool IsPublic,
    string? Player1,
    string? Player2,
    TimeSpan TimePerPlayer,
    TimeSpan TimeLeft1,
    TimeSpan TimeLeft2,
    string Fen,
    char? Result
)
{
    public GameDataDTO(GameData data) : this(
        data.GameId,
        data.Name,
        data.IsPublic,
        data.Player1,
        data.Player2,
        data.TimePerPlayer,
        data.TimeLeft1,
        data.TimeLeft2,
        data.Fen,
        data.Result)
    {
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

public record UserDataDTO
{
    public string Username { get; set; }
    public string Title { get; set; }
    public string TitleDescription { get; set; }
    public int SeasonsWon { get; set; }
    public int TournamentsWon { get; set; }
    public UserDataDTO(UserData userData)
    {
        Username = userData.Username;
        Title = userData.TitleHolder ? "Title holder" : userData.Stats.GetTitle().Item1;
        TitleDescription = userData.TitleHolder ? "The reigning champion" : userData.Stats.GetTitle().Item2;
        SeasonsWon = userData.SeasonsWon;
        TournamentsWon = userData.TournamentsWon;
    }
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
    public Move ToMove()
    {
        return new Move(ChessEngine.FieldDescToPos(From), ChessEngine.FieldDescToPos(To), Promotion);
    }
}

public record GameListDTO(List<GameDataDTO> Games)
{
    public GameListDTO(List<GameData> games) : this(
        games.Select(g => new GameDataDTO(g)).ToList()
    )
    { }
};
public record MoveListDTO
{
    public List<MoveDTO> Moves = [];

    public MoveListDTO(List<MoveDTO> moves)
    {
        Moves = moves;
    }
    public MoveListDTO(List<Move> moves)
    {
        foreach (var move in moves)
            Moves.Add(new(move));
    }
}