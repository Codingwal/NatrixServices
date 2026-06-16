using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Chess.Core.Interfaces;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Application.Commands;

public record DoMoveCommand(GameId GameId, Move Move) : ICommand;

public class DoMoveCommandHandler(IGameStorage GameStorage, IChessEngine ChessEngine) : ICommandHandler<DoMoveCommand>
{
    public async Task<Result> HandleAsync(DoMoveCommand command)
    {
        var game = await GameStorage.GetGameAsync(command.GameId);
        if (game == null) return new Error(ErrorType.NotFound, $"Game with id {command.GameId} not found!");

        if (!Fen.FenToBoard(game.Fen).TryGetValue(out ChessBoard? board, out var error))
            return error;

        bool isMoveLegal = ChessEngine.IsMoveLegal(board, command.Move);
        if (!isMoveLegal) return new Error(ErrorType.BadRequest, "Illegal move");

        board.DoMove(command.Move);
        GameResult? gameResult = ChessEngine.GetResult(board);
        string newFen = Fen.BoardToFen(board);

        game.DoMove(newFen, gameResult);

        await GameStorage.UpdateGameAsync(game);

        return Result.Success();
    }
}