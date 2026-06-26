using NatrixServices.Betting.Application.Commands;
using NatrixServices.Betting.Core.Entities;
using NatrixServices.Shared.Core;

namespace NatrixServices.Betting.Presentation.DTOs;

public record CreateMatchRequest(string Name, string Event, string Player1, string Player2, DateTime StartTime,
    float OddsPlayer1, float OddsDraw, float OddsPlayer2)
{
    public Result<CreateMatchCommand> ToCommand()
    {
        if (!MatchOdds.Create(OddsPlayer1, OddsDraw, OddsPlayer2).TryGetValue(out MatchOdds? odds, out var error))
            return error;

        return new CreateMatchCommand(Name, Event, Player1, Player2, StartTime, odds);
    }
}

public record MatchResultDTO(uint Player1, uint Player2)
{
    public Result<MatchResult> ToMatchResult()
    {
        return new MatchResult(Player1, Player2);
    }
}

public record MatchInfoDTO(string MatchId, string Name, string Event, string Player1, string Player2, DateTime StartTime,
    float OddsPlayer1, float OddsDraw, float OddsPlayer2);

public static class StatusDTO
{
    public static Result<bool?> ToOpenFilter(string? status)
    {
        if (status == null)
            return Result<bool?>.Success(null);
        else if (status == "open")
            return true;
        else if (status == "done")
            return false;
        else
            return new Error(ErrorType.BadRequest, $"Invalid status \"{status}\".");
    }
}