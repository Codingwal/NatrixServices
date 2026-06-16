using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;

namespace NatrixServices.Chess.Application.Events;

public record GameFinishedEvent(GameId GameId) : IEvent;