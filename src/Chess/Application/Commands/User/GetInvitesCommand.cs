using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Application.Commands;

public record GetInvitesCommand(string Player) : ICommand<IEnumerable<GameInvite>>;

public class GetInvitesCommandHandler(IUserStorage userStorage) : ICommandHandler<GetInvitesCommand, IEnumerable<GameInvite>>
{
    public async Task<Result<IEnumerable<GameInvite>>> HandleAsync(GetInvitesCommand command)
    {
        var user = await userStorage.GetUserAsync(command.Player);
        if (user == null) return new Error(ErrorType.NotFound, $"User with username \"{command.Player}\" not found.");

        return user.Invites;
    }
}