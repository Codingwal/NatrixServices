using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Core.Engine;

public class PawnMovementStrategy : IMovementStrategy
{
    private static readonly List<ChessFigure> promotionFigures = [ChessFigure.Rook, ChessFigure.Knight, ChessFigure.Bishop, ChessFigure.Queen];
    public IEnumerable<Move> GetPseudoLegalMoves(Int2 startPos, ChessBoard board)
    {
        ChessPiece piece = board.GetPiece(startPos);
        int moveDir = (piece.Color == Players.White) ? 1 : -1;
        int startRow = (piece.Color == Players.White) ? 1 : 6;
        int promotionRow = (piece.Color == Players.White) ? 7 : 0;

        List<Int2> dests = [];

        // Normal forward move
        {
            Int2 dest = startPos + new Int2(0, moveDir);
            if (board.GetPiece(dest).Figure == ChessFigure.None)
                dests.Add(dest);
        }

        // Double move
        if (startPos.y == startRow)
        {
            Int2 dest = startPos + new Int2(0, 2 * moveDir);
            if (board.GetPiece(startPos + new Int2(0, moveDir)).Figure == ChessFigure.None
                && board.GetPiece(dest).Figure == ChessFigure.None)
            {
                dests.Add(dest);
            }
        }

        // Capture
        foreach (var offset in new List<Int2>() { new(1, moveDir), new(-1, moveDir) })
        {
            Int2 pos = startPos + offset;
            if (!ChessBoard.InBounds(pos)) continue;

            ChessPiece capturedPiece = board.GetPiece(pos);
            if (capturedPiece.Figure != ChessFigure.None && !capturedPiece.IsColor(piece.Color!.Value))
                dests.Add(pos);
            else if (board.EnPassantTarget.HasValue && pos == board.EnPassantTarget.Value)
                dests.Add(pos);
        }

        // Return moves and handle promotion
        foreach (var dest in dests)
        {
            if (dest.y != promotionRow)
                yield return new Move(startPos, dest);
            else
            {
                foreach (var figure in promotionFigures)
                    yield return new Move(startPos, dest, figure);
            }
        }
    }
}