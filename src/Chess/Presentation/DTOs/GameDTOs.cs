using System.Text.Json.Serialization;
using NatrixServices.Chess.Core;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Presentation.DTOs;

public record GameDataDTO(
    string GameId,

    string Name,
    bool IsPublic,
    string Status,

    string? Player1,
    string? Player2,

    char NextPlayer,

    double TimePerPlayer,
    double TimeLeft1,
    double TimeLeft2,

    string Fen,

    char? Result
)
{
    public GameDataDTO(ChessGame data) : this(
        data.GameId.Value,

        data.Name,
        data.IsPublic,
        GameStatusDTO.StatusToStr(data.Status),

        data.PlayerWhite,
        data.PlayerBlack,

        (data.NextPlayer == Players.White) ? 'w' : 'b',

        data.TimePerPlayer.TotalMinutes,
        data.TimeLeftWhite.TotalMinutes,
        data.TimeLeftBlack.TotalMinutes,

        data.Fen,
        ConvertGameResult(data.MatchResult)
    )
    { }


    private static char? ConvertGameResult(GameResult? gameResult)
    {
        if (gameResult == null) return null;
        return gameResult switch
        {
            GameResult.WinWhite => 'w',
            GameResult.WinBlack => 'b',
            GameResult.Draw => 'd',
            _ => throw new ArgumentException()
        };
    }
}

public record ChessBoardDTO
{
    public char[][] Board { get; set; } = new char[8][];

    public ChessBoardDTO(ChessBoard board)
    {
        for (int y = 0; y < 8; y++)
        {
            Board[y] = new char[8];
            for (int x = 0; x < 8; x++)
            {
                Board[y][x] = Fen.PieceToChar(board.Fields[x, y]);
            }
        }
    }
}

[method: JsonConstructor]
public record MoveDTO(string From, string To, char? Promotion = null)
{
    public MoveDTO(Move move)
    : this(
        From: Fen.PosToFieldDesc(move.Origin),
        To: Fen.PosToFieldDesc(move.Destination),
        Promotion: (move.Promotion != null) ? Fen.PieceToChar(new(move.Promotion.Value, Players.Black)) : null
    )
    { }
    public Result<Move> ToMove()
    {
        var resultFrom = Fen.FieldDescToPos(From);
        if (resultFrom.IsFailure) return resultFrom.Error;

        var resultTo = Fen.FieldDescToPos(To);
        if (resultTo.IsFailure) return resultTo.Error;

        var resultPromotion = GetPromotionPiece(Promotion);
        if (resultPromotion.IsFailure) return resultPromotion.Error;

        return new Move(resultFrom.Value, resultTo.Value, resultPromotion.Value);
    }

    private static Result<ChessFigure?> GetPromotionPiece(char? c)
    {
        if (!c.HasValue)
            return Result<ChessFigure?>.Success(null);

        return Fen.CharToPiece(c.Value)
            .Map<ChessFigure?>(p => p.Figure);
    }
}

public record MoveListDTO(IEnumerable<MoveDTO> Moves)
{
    public MoveListDTO(IEnumerable<Move> moves)
        : this(moves.Select(m => new MoveDTO(m)))
    {
    }
}

public record GameListDTO(IEnumerable<GameDataDTO> Games)
{
    public GameListDTO(IEnumerable<ChessGame> games)
        : this(games.Select(g => new GameDataDTO(g)))
    {
    }
}

public static class GameStatusDTO
{
    public static string StatusToStr(GameStatus status)
    {
        return status switch
        {
            GameStatus.WaitingForPlayers => "waiting",
            GameStatus.Waiting => "scheduled",
            GameStatus.Active => "active",
            GameStatus.Done => "done",
            _ => throw new ArgumentException()
        };
    }

    public static Result<GameStatus> StrToStatus(string status)
    {
        return status switch
        {
            "waiting" => GameStatus.WaitingForPlayers,
            "scheduled" => GameStatus.Waiting,
            "active" => GameStatus.Active,
            "done" => GameStatus.Done,
            _ => new Error(ErrorType.BadRequest, $"Invalid status \"{status}\".")
        };
    }
}

public record DrawOfferDTO(string Player)
{
    public DrawOfferDTO(DrawOffer data)
        : this(data.Player)
    { }
}