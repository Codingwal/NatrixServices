using NatrixServices.Chess.Application.Events;
using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Chess.Core.Services;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Application.Commands;

public record DoMoveCommand(GameId GameId, Move Move, string Player) : ICommand;

public class DoMoveCommandHandler(IGameStorage gameStorage, IChessEngine chessEngine, IEventManager eventManager) : ICommandHandler<DoMoveCommand>
{
    public async Task<Result> HandleAsync(DoMoveCommand command)
    {
        var game = await gameStorage.GetGameAsync(command.GameId);
        if (game == null) return new Error(ErrorType.NotFound, $"Game with id {command.GameId} not found!");

        // If the game is waiting to be started and this player is the first player to move,
        // start the game (and then do the move)
        if (game.Status == GameStatus.Waiting && game.GetPlayer(command.Player) == Players.White)
            game.StartGame();

        if (!Fen.FenToBoard(game.Fen).TryGetValue(out ChessBoard? board, out var error))
            return error;

        bool isMoveLegal = chessEngine.IsMoveLegal(board, command.Move);
        if (!isMoveLegal) return new Error(ErrorType.BadRequest, "Illegal move");

        board.DoMove(command.Move);
        GameResult? gameResult = chessEngine.GetResult(board);
        string newFen = Fen.BoardToFen(board);

        if (game.DoMove(newFen, gameResult, command.Player).TryGetError(out error))
            return error;

        await gameStorage.UpdateGameAsync(game);

        if (game.MatchResult != null)
            await eventManager.PublishEventAsync(new GameFinishedEvent(command.GameId));

        return Result.Success();
    }
}