using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Core;
using EventId = NatrixServices.Chess.Core.Entities.EventId;

namespace NatrixServices.Chess.API;

public enum EventStatus { WaitingForPlayers, Waiting, Active, Done }
public enum EventType { Season, Tournament }
public class ChessEvent(EventId eventId, string name, bool isPublic, EventType eventType, uint minPlayerCount, uint maxPlayerCount, TimeSpan timePerPlayer)
{
    public EventId EventId { get; } = eventId;

    public string Name { get; } = name;
    public bool IsPublic { get; } = isPublic;
    public EventStatus Status { get; private set; } = EventStatus.WaitingForPlayers;
    public EventType EventType { get; } = eventType;

    public uint MinPlayerCount { get; } = minPlayerCount;
    public uint MaxPlayerCount { get; } = maxPlayerCount;
    public List<string> Players { get; } = [];

    public TimeSpan TimePerPlayer { get; } = timePerPlayer;
    public DateTime? StartTime { get; private set; } = null;

    public List<GameId> Games { get; } = [];

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



        return Result.Success();
    }
}