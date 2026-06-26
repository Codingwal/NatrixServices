using NatrixServices.Betting.Core.Entities;

namespace NatrixServices.Betting.Application.Services;

public interface IBetStorage
{
    public Task AddBetAsync(Bet bet);
    public Task<IEnumerable<Bet>> GetBetsAsync(string? owner = null, MatchId? matchId = null, bool? open = null);
    public Task UpdateBetAsync(Bet bet);
    public Task UpdateBetsAsync(IEnumerable<Bet> bets);
}