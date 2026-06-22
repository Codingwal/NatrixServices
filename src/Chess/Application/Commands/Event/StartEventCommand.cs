using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;
using NatrixServices.Chess.Core.Entities;

using EventId = NatrixServices.Chess.Core.Entities.EventId;
using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Services;

namespace NatrixServices.Chess.Application.Commands;

public record StartEventCommand(EventId EventId) : ICommand;

public class StartEventCommandHandler(IEventStorage eventStorage, IGameStorage gameStorage, IMatchGenerator matchGenerator) : ICommandHandler<StartEventCommand>
{
    public async Task<Result> HandleAsync(StartEventCommand command)
    {
        ChessEvent? chessEvent = await eventStorage.GetEventAsync(command.EventId);
        if (chessEvent == null) return new Error(ErrorType.NotFound, $"Event with id {command.EventId} not found.");

        if (chessEvent.StartEvent().TryGetError(out var error))
            return error;

        var games = matchGenerator.GenerateGames(chessEvent);
        await gameStorage.AddGamesAsync(games);

        return Result.Success();
    }
}