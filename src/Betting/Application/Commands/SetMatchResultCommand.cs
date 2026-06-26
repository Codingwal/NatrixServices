using NatrixServices.Betting.Application.Events;
using NatrixServices.Betting.Application.Services;
using NatrixServices.Betting.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Betting.Application.Commands;

public record SetMatchResultCommand(MatchId MatchId, MatchResult MatchResult) : ICommand;

public class SetMatchResultCommandHandler(IMatchStorage matchStorage, IEventManager eventManager) : ICommandHandler<SetMatchResultCommand>
{
    public async Task<Result> HandleAsync(SetMatchResultCommand command)
    {
        Match? match = await matchStorage.GetMatchAsync(command.MatchId);
        if (match == null) return new Error(ErrorType.NotFound, $"Could not find match with id {command.MatchId}.");

        if (match.StartTime > DateTime.UtcNow)
            return new Error(ErrorType.BadRequest, $"The match has not started yet.");

        match.Result = command.MatchResult;

        await matchStorage.UpdateMatchAsync(match);

        await eventManager.PublishEventAsync(new MatchFinishedEvent(command.MatchId, command.MatchResult));

        return Result.Success();
    }
}