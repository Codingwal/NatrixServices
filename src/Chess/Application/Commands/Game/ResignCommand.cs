using NatrixServices.Chess.Application.Events;
using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Application.Commands;

public record ResignCommand(GameId GameId, string Player) : ICommand;

public class ResignCommandHandler(IGameStorage GameStorage, IEventManager eventManager) : ICommandHandler<ResignCommand>
{
    public async Task<Result> HandleAsync(ResignCommand command)
    {
        var game = await GameStorage.GetGameAsync(command.GameId);
        if (game == null)
            return new Error(ErrorType.NotFound, $"Game with id {command.GameId} not found!");

        if (game.Resign(command.Player).TryGetError(out var error))
            return error;

        await GameStorage.UpdateGameAsync(game);

        if (game.MatchResult != null)
            await eventManager.PublishEventAsync(new GameFinishedEvent(command.GameId));

        return Result.Success();
    }
}