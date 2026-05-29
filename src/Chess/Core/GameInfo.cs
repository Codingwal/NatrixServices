global using GameId = string;

namespace NatrixServices.Chess.Core;

public class GameInfo
{
    public required GameId GameId { get; set; }
    public required string Name { get; set; }

    public required bool IsPublic { get; set; }

    public string? Player1 { get; set; } = null;
    public string? Player2 { get; set; } = null;

    public required TimeSpan TimePerPlayer { get; set; }
    public required TimeSpan TimeLeft1 { get; set; }
    public required TimeSpan TimeLeft2 { get; set; }
    public DateTimeOffset LastMoveTime { get; set; } = default;

    public required string Fen { get; set; }
    public List<Move> Moves { get; set; } = [];

    public char? Result { get; set; } = null; // optional. 'w' => white won, 'b' => black won, 'd' => draw


    public bool Waiting => Player1 == null || Player2 == null;
    public bool Done => Result != null;
    public bool Active => !Waiting && !Done;
    public bool Scheduled => false; // TODO: Update when scheduling is implemented

    public string Status
    {
        get
        {
            if (Waiting)
                return "waiting";
            else if (Done)
                return "done";
            else if (Active)
                return "active";
            else if (Scheduled)
                return "scheduled";
            else
                throw new();
        }
    }

    public static GameInfo CreateGameInfo(GameId gameId, string name, bool isPublic, int timePerPlayerMinutes, string fen)
    {
        TimeSpan timePerPlayer = TimeSpan.FromMinutes(timePerPlayerMinutes);
        return new GameInfo()
        {
            GameId = gameId,
            Name = name,
            IsPublic = isPublic,
            TimePerPlayer = timePerPlayer,
            TimeLeft1 = timePerPlayer,
            TimeLeft2 = timePerPlayer,
            Fen = fen
        };
    }
}