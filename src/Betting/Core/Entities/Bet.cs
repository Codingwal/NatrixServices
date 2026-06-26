namespace NatrixServices.Betting.Core.Entities;

public record Bet(MatchId MatchId, string Owner, MatchResult ExpectedResult, uint Stake)
{
    public bool Done { get; set; } = false;

    private Bet() : this(default!, default!, default!, default!) { } // Needed by EF Core
}