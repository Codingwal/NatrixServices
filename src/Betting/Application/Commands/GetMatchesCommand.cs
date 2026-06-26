using NatrixServices.Betting.Application.Services;
using NatrixServices.Betting.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Betting.Application.Commands;

using MatchList = IEnumerable<Match>;

public record GetMatchesCommand(bool? Open, string? Event) : ICommand<MatchList>;

public class GetMatchesCommandHandler(IMatchStorage matchStorage) : ICommandHandler<GetMatchesCommand, MatchList>
{
    public async Task<Result<MatchList>> HandleAsync(GetMatchesCommand command)
    {
        MatchList matches = await matchStorage.GetMatchesAsync(command.Open, command.Event);

        return Result<MatchList>.Success(matches);
    }
}