using NatrixServices.Chess.Core.Entities;

namespace NatrixServices.Chess.Core.Services;

public interface IMatchGenerator
{
    public IEnumerable<ChessGame> GenerateGames(ChessEvent chessEvent);
}