using NatrixServices.Betting.Core.Entities;

namespace NatrixServices.Betting.Presentation.DTOs;

public record BetDTO(string MatchId, uint Stake, uint Player1, uint Player2)
{
    public BetDTO(Bet data)
        : this(data.MatchId.Value, data.Stake, data.ExpectedResult.Player1, data.ExpectedResult.Player2)
    {
    }
}

public record BetListDTO(IEnumerable<BetDTO> Bets)
{
    public BetListDTO(IEnumerable<Bet> bets)
        : this(bets.Select(b => new BetDTO(b)))
    {
    }
}