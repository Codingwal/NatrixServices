using NatrixServices.Betting.Application.Services;
using NatrixServices.Betting.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Betting.Application.Commands;

public record CreateMatchCommand(string Name, string Event, string Player1, string Player2, DateTime StartTime, MatchOdds Odds) : ICommand<MatchId>;

public class CreateMatchCommandHandler(IMatchStorage matchStorage) : ICommandHandler<CreateMatchCommand, MatchId>
{
    public async Task<Result<MatchId>> HandleAsync(CreateMatchCommand command)
    {
        if (!StandardChars.IsAllowed(command.Name))
            return new Error(ErrorType.BadRequest, $"Illegal name \"{command.Name}\".");

        if (!StandardChars.IsAllowed(command.Player1))
            return new Error(ErrorType.BadRequest, $"Illegal name \"{command.Player1}\".");

        if (!StandardChars.IsAllowed(command.Player2))
            return new Error(ErrorType.BadRequest, $"Illegal name \"{command.Player2}\".");

        if (command.StartTime < DateTime.UtcNow)
            return new Error(ErrorType.BadRequest, $"The match time is in the past");

        MatchId matchId = MatchId.Generate();
        var match = new Match(matchId, command.Name, command.Event, command.Player1, command.Player2, command.StartTime, command.Odds);

        await matchStorage.AddMatchAsync(match);

        return matchId;
    }
}