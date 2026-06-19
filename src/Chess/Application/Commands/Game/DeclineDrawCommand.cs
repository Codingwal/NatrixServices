using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Application.Commands;

public record DeclineDrawCommand(GameId GameId, string Player) : ICommand;

public class DeclineDrawCommandHandler(IGameStorage gameStorage) : ICommandHandler<DeclineDrawCommand>
{
    public async Task<Result> HandleAsync(DeclineDrawCommand command)
    {
        var game = await gameStorage.GetGameAsync(command.GameId);
        if (game == null)
            return new Error(ErrorType.NotFound, $"Game with id {command.GameId} not found!");

        if (game.DeclineDraw(command.Player).TryGetError(out var error))
            return error;

        await gameStorage.UpdateGameAsync(game);

        return Result.Success();
    }
}