using System.ComponentModel.DataAnnotations;
using NatrixServices.Chess.Core;

namespace NatrixServices.Chess.Data;

public class GameData : IIdentifiable<GameId>
{
    [Key, Required, StringLength(8)]
    public required GameId Id { get; set; }
    public required string Name { get; set; }

    public bool IsPublic { get; set; }

    public required string? Player1 { get; set; }
    public required string? Player2 { get; set; }

    public required TimeSpan TimePerPlayer { get; set; }
    public required TimeSpan TimeLeft1 { get; set; }
    public required TimeSpan TimeLeft2 { get; set; }
    public required DateTimeOffset LastMoveTime { get; set; }

    public required string Fen { get; set; }
    public required List<Move> Moves { get; set; }

    public char? Result { get; set; }

    public GameInfo ToGameInfo()
    {
        return new GameInfo()
        {
            GameId = Id,
            Name = Name,

            IsPublic = IsPublic,

            Player1 = Player1,
            Player2 = Player2,

            TimePerPlayer = TimePerPlayer,
            TimeLeft1 = TimeLeft1,
            TimeLeft2 = TimeLeft2,
            LastMoveTime = LastMoveTime,

            Fen = Fen,
            Moves = Moves,

            Result = Result
        };
    }

    public static GameData FromGameInfo(GameInfo game)
    {
        return new GameData()
        {
            Id = game.GameId,
            Name = game.Name,

            IsPublic = game.IsPublic,

            Player1 = game.Player1,
            Player2 = game.Player2,

            TimePerPlayer = game.TimePerPlayer,
            TimeLeft1 = game.TimeLeft1,
            TimeLeft2 = game.TimeLeft2,
            LastMoveTime = game.LastMoveTime,

            Fen = game.Fen,
            Moves = game.Moves,

            Result = game.Result
        };
    }
}