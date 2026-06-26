using NatrixServices.Betting.Core.Entities;

namespace NatrixServices.Betting.Core.Services;

public interface IProfitCalculator
{
    public float CalculatePayout(MatchResult result, MatchResult expectedResult, uint stake, MatchOdds odds);
}