using NatrixServices.Chess.Core.Entities;
using NatrixServices.Chess.Core.Interfaces;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Core.Engine;

public class ChessEngine : IChessEngine
{
    public IEnumerable<Move> GetAllLegalMoves(ChessBoard board)
    {
        List<Move> moves = [];
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                moves.AddRange(GetLegalMoves(board, new Int2(x, y)));
            }
        }
        return moves;
    }
    public IEnumerable<Move> GetLegalMoves(ChessBoard board, Int2 startPos)
    {
        if (!ChessBoard.InBounds(startPos))
            return [];

        ChessPiece piece = board.GetPiece(startPos);

        if (piece.Figure == ChessFigure.None)
            return [];
        if (piece.Color!.Value != board.NextPlayer)
            return [];

        IMovementStrategy strategy = MovementFactory.GetMovementStrategy(piece.Figure);
        var pseudoLegalMoves = strategy.GetPseudoLegalMoves(startPos, board);

        return pseudoLegalMoves.Where(move =>
        {
            var boardCopy = board.Copy();
            boardCopy.DoMove(move);
            return !InCheck(boardCopy, piece.Color!.Value);
        });
    }

    public GameResult? GetResult(ChessBoard board)
    {
        if (board.HalfMovesSinceAction >= 100)
            return GameResult.Draw;

        // Handle no checkmate and stalemate
        if (!GetAllLegalMoves(board).Any())
        {
            if (InCheck(board, board.NextPlayer))
                return (board.NextPlayer == Players.White) ? GameResult.WinBlack : GameResult.WinWhite;
            else
                return GameResult.Draw;
        }

        return null;
    }

    public bool IsMoveLegal(ChessBoard board, Move move)
    {
        return GetLegalMoves(board, move.Origin).Contains(move);
    }
    private bool InCheck(ChessBoard board, Players player)
    {
        Int2? kingPos = board.FindPiece(new(ChessFigure.King, player)) ?? throw new Exception($"Can't find king of player {player}");

        return GetAllLegalMoves(board).Any(move => move.Destination == kingPos);
    }
}