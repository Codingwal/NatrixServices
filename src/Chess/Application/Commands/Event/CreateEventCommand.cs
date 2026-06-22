using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;
using NatrixServices.Chess.Core.Entities;

using EventId = NatrixServices.Chess.Core.Entities.EventId;
using NatrixServices.Chess.Application.Interfaces;

namespace NatrixServices.Chess.Application.Commands;

public record CreateEventCommand(string Name, bool IsPublic, EventType EventType,
    uint MinPlayerCount, uint MaxPlayerCount, TimeSpan TimePerPlayer, TimeSpan TotalEventDuration)
    : ICommand<EventId>;

public class CreateEventCommandHandler(IEventStorage eventStorage) : ICommandHandler<CreateEventCommand, EventId>
{
    public async Task<Result<EventId>> HandleAsync(CreateEventCommand command)
    {
        if (!StandardChars.IsAllowed(command.Name))
            return new Error(ErrorType.BadRequest, $"Event name \"{command.Name}\" contains illegal characters.");

        EventId eventId = EventId.Generate();

        var result = ChessEvent.CreateEvent(eventId, command.Name, command.IsPublic, command.EventType,
            command.MinPlayerCount, command.MaxPlayerCount, command.TimePerPlayer, command.TotalEventDuration);

        if (result.IsFailure) return result.Error;
        ChessEvent chessEvent = result.Value;

        await eventStorage.AddEventAsync(chessEvent);

        return eventId;
    }
}

