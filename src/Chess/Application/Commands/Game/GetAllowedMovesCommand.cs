using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Chess.Core.Services;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Application.Commands;

using MoveList = IEnumerable<Move>;

public record GetAllowedMovesCommand(GameId GameId, Int2? StartPos) : ICommand<MoveList>;

public class GetAllowedMovesCommandHandler(IGameStorage GameStorage, IChessEngine ChessEngine)
    : ICommandHandler<GetAllowedMovesCommand, MoveList>
{
    public async Task<Result<MoveList>> HandleAsync(GetAllowedMovesCommand command)
    {
        var game = await GameStorage.GetGameAsync(command.GameId);
        if (game == null) return new Error(ErrorType.NotFound, $"Game with id {command.GameId} not found!");

        if (!Fen.FenToBoard(game.Fen).TryGetValue(out ChessBoard? board, out var error))
            return error;

        if (command.StartPos.HasValue)
            return Result<MoveList>.Success(ChessEngine.GetLegalMoves(board, command.StartPos.Value));
        else
            return Result<MoveList>.Success(ChessEngine.GetAllLegalMoves(board));

    }
}