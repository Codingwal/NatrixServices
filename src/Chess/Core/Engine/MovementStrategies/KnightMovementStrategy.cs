using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Core.Engine;

public class KnightMovementStrategy : IMovementStrategy
{
    private static readonly List<Int2> destOffsets = [
        new(2, 1), new(1, 2), new(-1, 2), new(-2, 1),
        new(-2, -1), new(-1, -2), new(1, -2), new(2, -1)
    ];

    public IEnumerable<Move> GetPseudoLegalMoves(Int2 startPos, ChessBoard board)
    {
        Players player = board.GetPiece(startPos).Color!.Value;

        foreach (Int2 offset in destOffsets)
        {
            Int2 dest = startPos + offset;

            if (!ChessBoard.InBounds(dest)) continue;

            // Can't capture your own piece
            ChessPiece destPiece = board.GetPiece(dest);
            if (destPiece.IsColor(player)) continue;

            yield return new Move(startPos, dest);
        }
    }
}