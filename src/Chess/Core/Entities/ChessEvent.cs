using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Core.Entities;

public enum EventStatus { WaitingForPlayers, Waiting, Active, Done }
public enum EventType { Season, Tournament }

public record ChessEvent(
    EventId EventId,

    string Name,
    bool IsPublic,
    EventType EventType,

    uint MinPlayerCount,
    uint MaxPlayerCount,

    TimeSpan TimePerPlayer,
    TimeSpan TotalEventDuration
)
{
    public EventStatus Status { get; private set; } = EventStatus.WaitingForPlayers;
    public List<string> Players { get; } = [];
    public DateTime? StartTime { get; private set; } = null;

    public static Result<ChessEvent> CreateEvent(EventId eventId, string name, bool isPublic, EventType eventType,
        uint minPlayerCount, uint maxPlayerCount, TimeSpan timePerPlayer, TimeSpan totalEventDuration)
    {
        if (minPlayerCount > maxPlayerCount)
            return new Error(ErrorType.BadRequest, $"MinPlayerCount ({minPlayerCount}) must not be greater than MaxPlayerCount ({maxPlayerCount})");

        if (minPlayerCount < 2)
            return new Error(ErrorType.BadRequest, $"MinPlayerCount ({minPlayerCount}) must be at least 2");

        return new ChessEvent(eventId, name, isPublic, eventType, minPlayerCount, maxPlayerCount, timePerPlayer, totalEventDuration);
    }

    public Result JoinEvent(string playerName)
    {
        if (Status != EventStatus.WaitingForPlayers)
            return new Error(ErrorType.Conflict, "Event is currently not joinable.");

        if (Players.Contains(playerName))
            return new Error(ErrorType.Forbidden, $"Player \"{playerName}\" is already a participant.");

        Players.Add(playerName);

        if (Players.Count >= MaxPlayerCount)
            Status = EventStatus.Waiting;

        return Result.Success();
    }

    public Result StartEvent()
    {
        if (Status != EventStatus.Waiting && Status != EventStatus.WaitingForPlayers)
            return new Error(ErrorType.Conflict, $"Cannot start an event with status {Status}.");

        if (Players.Count < MinPlayerCount)
            return new Error(ErrorType.Conflict, $"Too few players!");

        Status = EventStatus.Active;
        StartTime = DateTime.UtcNow;

        return Result.Success();
    }
}


// public class ChessEvent
// {
//     public EventId EventId { get; }

//     public string Name { get; }
//     public bool IsPublic { get; }
//     public EventStatus Status { get; private set; }
//     public EventType EventType { get; }

//     public uint MinPlayerCount { get; }
//     public uint MaxPlayerCount { get; }
//     public List<string> Players { get; } = [];

//     public TimeSpan TimePerPlayer { get; }
//     public TimeSpan TotalEventDuration { get; }

//     public DateTime? StartTime { get; private set; }

//     private ChessEvent(EventId eventId, string name, bool isPublic, EventType eventType,
//         uint minPlayerCount, uint maxPlayerCount, TimeSpan timePerPlayer, TimeSpan totalEventDuration)
//     {
//         EventId = eventId;
//         Name = name;
//         IsPublic = isPublic;
//         // ...
//     }

//     public static Result<ChessEvent> CreateEvent(EventId eventId, string name, bool isPublic, EventType eventType,
//         uint minPlayerCount, uint maxPlayerCount, TimeSpan timePerPlayer, TimeSpan totalEventDuration)
//     {
//         if (minPlayerCount > maxPlayerCount)
//             return new Error(ErrorType.BadRequest, $"MinPlayerCount ({minPlayerCount}) must not be greater than MaxPlayerCount ({maxPlayerCount})");

//         if (minPlayerCount < 2)
//             return new Error(ErrorType.BadRequest, $"MinPlayerCount ({minPlayerCount}) must be at least 2");

//         return new ChessEvent(eventId, name, isPublic, eventType, minPlayerCount, maxPlayerCount, timePerPlayer, totalEventDuration);
//     }

//     public Result JoinEvent(string playerName)
//     {
//         if (Status != EventStatus.WaitingForPlayers)
//             return new Error(ErrorType.Conflict, "Event is currently not joinable.");

//         if (Players.Contains(playerName))
//             return new Error(ErrorType.Forbidden, $"Player \"{playerName}\" is already a participant.");

//         Players.Add(playerName);

//         if (Players.Count >= MaxPlayerCount)
//             Status = EventStatus.Waiting;

//         return Result.Success();
//     }

//     public Result StartEvent()
//     {
//         if (Status != EventStatus.Waiting && Status != EventStatus.WaitingForPlayers)
//             return new Error(ErrorType.Conflict, $"Cannot start an event with status {Status}.");

//         if (Players.Count < MinPlayerCount)
//             return new Error(ErrorType.Conflict, $"Too few players!");

//         Status = EventStatus.Active;
//         StartTime = DateTime.UtcNow;

//         return Result.Success();
//     }
// }