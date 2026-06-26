using NatrixServices.Betting.Core.Entities;
using NatrixServices.Shared.Application;

namespace NatrixServices.Betting.Application.Events;

public record MatchFinishedEvent(MatchId MatchId, MatchResult MatchResult) : IEvent;