using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Core.Engine;

public class SlidingMovementStrategy : IMovementStrategy
{
    private readonly List<Int2> directions = [];
    public SlidingMovementStrategy(bool straight, bool diagonal)
    {
        if (straight)
            directions.AddRange([new(1, 0), new(0, 1), new(-1, 0), new(0, -1)]);

        if (diagonal)
            directions.AddRange([new(1, 1), new(-1, 1), new(-1, -1), new(1, -1)]);
    }
    public IEnumerable<Move> GetPseudoLegalMoves(Int2 startPos, ChessBoard board)
    {
        Players player = board.GetPiece(startPos).Color!.Value;

        foreach (Int2 dir in directions)
        {
            for (Int2 pos = startPos + dir; ChessBoard.InBounds(pos); pos += dir)
            {
                ChessPiece destPiece = board.GetPiece(pos);

                // Can't capture your own piece
                if (destPiece.IsColor(player))
                    break;

                yield return new Move(startPos, pos);

                // Can't jump over pieces
                if (destPiece.Figure != ChessFigure.None)
                    break;
            }
        }
    }
}