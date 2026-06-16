using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Core.Engine;

public interface IMovementStrategy
{
    public IEnumerable<Move> GetPseudoLegalMoves(Int2 startPos, ChessBoard board);
}