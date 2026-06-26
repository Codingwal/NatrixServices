using NatrixServices.Betting.Application.Services;
using NatrixServices.Betting.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Betting.Application.Commands;

public record PlaceBetCommand(string Username, MatchId MatchId, MatchResult ExpectedResult, uint Stake) : ICommand;

public class PlaceBetCommandHandler(IMatchStorage matchStorage, IBetStorage betStorage, IUserStorage userStorage) : ICommandHandler<PlaceBetCommand>
{
    public async Task<Result> HandleAsync(PlaceBetCommand command)
    {
        Match? match = await matchStorage.GetMatchAsync(command.MatchId);
        if (match == null) return new Error(ErrorType.NotFound, $"Could not find match with id {command.MatchId}.");

        UserData? userData = await userStorage.GetUserAsync(command.Username);
        if (userData == null) return new Error(ErrorType.NotFound, $"Could not find user with name \"{command.Username}\".");

        if (match.StartTime < DateTime.UtcNow)
            return new Error(ErrorType.Conflict, $"Match with id {match.MatchId} is already done or active.");

        if (userData.Balance <= command.Stake)
            return new Error(ErrorType.Forbidden, $"Not have enough balance ({userData.Balance} / {command.Stake}).");

        if ((await betStorage.GetBetsAsync(command.Username, command.MatchId)).Any())
            return new Error(ErrorType.Conflict, $"You cannot place multiple bets on the same match.");

        var bet = new Bet(command.MatchId, command.Username, command.ExpectedResult, command.Stake);
        await betStorage.AddBetAsync(bet);

        userData.Balance -= command.Stake;

        await userStorage.UpdateUserAsync(userData);

        return Result.Success();
    }
}