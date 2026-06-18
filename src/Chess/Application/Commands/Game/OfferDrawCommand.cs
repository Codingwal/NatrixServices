using NatrixServices.Chess.Application.Events;
using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Application.Commands;

public record OfferDrawCommand(GameId GameId, string Player) : ICommand;

public class OfferDrawCommandHandler(IGameStorage gameStorage, IEventManager eventManager) : ICommandHandler<OfferDrawCommand>
{
    public async Task<Result> HandleAsync(OfferDrawCommand command)
    {
        var game = await gameStorage.GetGameAsync(command.GameId);
        if (game == null)
            return new Error(ErrorType.NotFound, $"Game with id {command.GameId} not found!");

        if (game.OfferDraw(command.Player).TryGetError(out var error))
            return error;

        await gameStorage.UpdateGameAsync(game);

        if (game.MatchResult != null)
            await eventManager.PublishEventAsync(new GameFinishedEvent(command.GameId));

        return Result.Success();
    }
}