using NatrixServices.Betting.Core.Entities;
using NatrixServices.Betting.Core.Services;

namespace NatrixServices.Betting.Core.ProfitCalculator;

enum Winner
{
    Player1,
    Player2,
    Draw
}

public class ProfitCalculator : IProfitCalculator
{
    public float CalculatePayout(MatchResult result, MatchResult expectedResult, uint stake, MatchOdds odds)
    {
        Winner winner = GetWinner(result);
        Winner expectedWinner = GetWinner(expectedResult);

        float multiplier;
        if (winner == expectedWinner)
        {
            if (winner == Winner.Player1)
                multiplier = odds.Player1;
            else if (winner == Winner.Player2)
                multiplier = odds.Player2;
            else
                multiplier = odds.Draw;
        }
        else
            multiplier = 0;

        if (result == expectedResult)
            multiplier *= 1.5f;

        return stake * multiplier;
    }

    private static Winner GetWinner(MatchResult result)
    {
        if (result.Player1 > result.Player2)
            return Winner.Player1;
        else if (result.Player1 < result.Player2)
            return Winner.Player2;
        else
            return Winner.Draw;
    }
}