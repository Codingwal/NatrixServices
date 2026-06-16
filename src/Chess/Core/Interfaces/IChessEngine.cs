using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Core.Interfaces;

public interface IChessEngine
{
    public bool IsMoveLegal(ChessBoard board, Move move);
    public GameResult? GetResult(ChessBoard board);
    public IEnumerable<Move> GetAllLegalMoves(ChessBoard board);
    public IEnumerable<Move> GetLegalMoves(ChessBoard board, Int2 startPos);
}