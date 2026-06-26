using NatrixServices.Betting.Application.Events;
using NatrixServices.Betting.Application.Services;
using NatrixServices.Betting.Core.Entities;
using NatrixServices.Betting.Core.Services;
using NatrixServices.Shared.Application;

namespace NatrixServices.Betting.Application.EventHandlers;

public class UpdateBets(IMatchStorage matchStorage, IBetStorage betStorage, IUserStorage userStorage, IProfitCalculator profitCalculator)
    : IEventHandler<MatchFinishedEvent>
{
    public async Task HandleAsync(MatchFinishedEvent e)
    {
        var match = await matchStorage.GetMatchAsync(e.MatchId)
            ?? throw new KeyNotFoundException($"Could not find match \"{e.MatchId}\"");

        var bets = await betStorage.GetBetsAsync(matchId: e.MatchId);

        List<UserData> usersToUpdate = [];
        foreach (var bet in bets)
        {
            bet.Done = true;

            var user = await userStorage.GetUserAsync(bet.Owner)
                ?? throw new KeyNotFoundException($"Could not find user \"{bet.Owner}\"");

            float payout = profitCalculator.CalculatePayout(e.MatchResult, bet.ExpectedResult, bet.Stake, match.Odds);

            user.Balance += payout;

            usersToUpdate.Add(user);
        }

        await betStorage.UpdateBetsAsync(bets);
        await userStorage.UpdateUsersAsync(usersToUpdate);
    }
}