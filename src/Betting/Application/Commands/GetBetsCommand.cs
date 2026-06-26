using NatrixServices.Betting.Application.Services;
using NatrixServices.Betting.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Betting.Application.Commands;

using BetList = IEnumerable<Bet>;

public record GetBetsCommand(string? Username, MatchId? MatchId, bool? Open) : ICommand<BetList>;

public class GetBetsCommandHandler(IBetStorage betStorage) : ICommandHandler<GetBetsCommand, BetList>
{
    public async Task<Result<BetList>> HandleAsync(GetBetsCommand command)
    {
        var bets = await betStorage.GetBetsAsync(command.Username, command.MatchId, command.Open);

        return Result<BetList>.Success(bets);
    }
}