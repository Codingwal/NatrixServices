using NatrixServices.Betting.Core.Entities;

namespace NatrixServices.Betting.Application.Services;

public interface IMatchStorage
{
    public Task AddMatchAsync(Match match);
    public Task<Match?> GetMatchAsync(MatchId matchId);
    public Task<IEnumerable<Match>> GetAllMatchesAsync();
    public Task<IEnumerable<Match>> GetMatchesAsync(bool? open, string? Event);
    public Task UpdateMatchAsync(Match match);
}