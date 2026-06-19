using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;

namespace NatrixServices.Chess.Application.Events;

public record GameFinishedEvent(GameId GameId) : IEvent;
public record PlayerInvitedEvent(GameId GameId, string Host, string Player) : IEvent;