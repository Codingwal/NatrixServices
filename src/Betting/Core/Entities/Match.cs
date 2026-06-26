using NatrixServices.Shared.Core;

namespace NatrixServices.Betting.Core.Entities;

public record Match(MatchId MatchId, string Name, string Event, string Player1, string Player2, DateTime StartTime, MatchOdds Odds)
{
    public MatchResult? Result { get; set; } = null;

    private Match() : this(default!, default!, default!, default!, default!, default!, default!) { } // Needed by EF Core
}

public record MatchResult(uint Player1, uint Player2);

// 2.0 <-> 50% probability
public record MatchOdds
{
    public float Player1 { get; init; }
    public float Draw { get; init; }
    public float Player2 { get; init; }

    private MatchOdds() { }

    private MatchOdds(float player1, float draw, float player2)
    {
        Player1 = player1;
        Draw = draw;
        Player2 = player2;
    }

    public static Result<MatchOdds> Create(float player1, float draw, float player2)
    {
        if (player1 <= 1 || draw <= 1 || player2 <= 1)
            return new Error(ErrorType.BadRequest, $"All odds must be greater than one");

        float probabilitySum = (1 / player1) + (1 / draw) + (1 / player2);

        if (probabilitySum < 1)
            return new Error(ErrorType.BadRequest, $"Probability sum must be at least 100%");

        if (probabilitySum > 1.1)
            return new Error(ErrorType.BadRequest, $"Probability sum must be under 110%");

        return new MatchOdds(player1, draw, player2);
    }
}