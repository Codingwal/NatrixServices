using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Core.Engine;

public class KingMovementStrategy : IMovementStrategy
{
    public IEnumerable<Move> GetPseudoLegalMoves(Int2 startPos, ChessBoard board)
    {
        Players player = board.GetPiece(startPos).Color!.Value;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Int2 dest = startPos + new Int2(x, y);

                if (!ChessBoard.InBounds(dest)) continue;

                // Can't capture your own piece
                ChessPiece destPiece = board.GetPiece(dest);
                if (destPiece.IsColor(player)) continue;

                yield return new Move(startPos, dest);
            }
        }
    }
}