using NatrixServices.Chess.Core.Entities;

namespace NatrixServices.Chess.Core.Engine;

public static class MovementFactory
{
    private static readonly Dictionary<ChessFigure, IMovementStrategy> Strategies = new() {
        {ChessFigure.Rook, new SlidingMovementStrategy(straight: true, diagonal: false)},
        {ChessFigure.Knight, new KnightMovementStrategy()},
        {ChessFigure.Bishop, new SlidingMovementStrategy(straight: false, diagonal: true)},
        {ChessFigure.Queen, new SlidingMovementStrategy(straight: true, diagonal: true)},
        {ChessFigure.King, new KingMovementStrategy()},
        {ChessFigure.Pawn, new PawnMovementStrategy ()},
    };

    public static IMovementStrategy GetMovementStrategy(ChessFigure figure)
    {
        if (Strategies.TryGetValue(figure, out var strategy))
            return strategy;
        else
            throw new Exception($"Missing movement strategy for ChessFigure {figure}");
    }
}