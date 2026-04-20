global using GameId = string;

using System.ComponentModel.DataAnnotations;
using NatrixServices.Chess.Core;

namespace NatrixServices.Chess.Data;

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