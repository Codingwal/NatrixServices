using NatrixServices.Chess.API;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;
using NatrixServices.Chess.Core.Entities;

using EventId = NatrixServices.Chess.Core.Entities.EventId;

namespace NatrixServices.Chess.Application.Commands;

public record CreateEventCommand(string Name, bool IsPublic, EventType EventType,
    uint MinPlayerCount, uint MaxPlayerCount, TimeSpan TimePerPlayer)
    : ICommand<EventId>;

public class CreateEventCommandHandler() : ICommandHandler<CreateEventCommand, EventId>
{
    public async Task<Result<EventId>> HandleAsync(CreateEventCommand command)
    {
        if (!StandardChars.IsAllowed(command.Name))
            return new Error(ErrorType.BadRequest, $"Event name \"{command.Name}\" contains illegal characters.");

        if (command.MinPlayerCount > command.MaxPlayerCount)
            return new Error(ErrorType.BadRequest, $"MinPlayerCount ({command.MinPlayerCount}) must not be greater than MaxPlayerCount ({command.MaxPlayerCount})");

        EventId eventId = EventId.Generate();

        ChessEvent chessEvent = new(eventId, command.Name, command.IsPublic, command.EventType,
            command.MinPlayerCount, command.MaxPlayerCount, command.TimePerPlayer);

        // TODO: use storage


        return eventId;
    }
}

