using NatrixServices.Chess.Application.Events;
using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Application.Commands;

public record InvitePlayerCommand(GameId GameId, string Host, string Player) : ICommand;

public class InvitePlayerCommandHandler(IUserStorage userStorage, IEventManager eventManager) : ICommandHandler<InvitePlayerCommand>
{
    public async Task<Result> HandleAsync(InvitePlayerCommand command)
    {
        if (command.Host == command.Player)
            return new Error(ErrorType.BadRequest, $"You cannot invite yourself to game {command.GameId}.");

        var user = await userStorage.GetUserAsync(command.Player);
        if (user == null) return new Error(ErrorType.NotFound, $"User with username \"{command.Player}\" not found.");

        if (user.Invites.Any(invite => invite.GameId == command.GameId))
            return new Error(ErrorType.Conflict, $"User \"{command.Player}\" has already been invited to game {command.GameId}");

        user.Invites.Add(new GameInvite(command.GameId, command.Host));

        await userStorage.UpdateUserAsync(user);

        await eventManager.PublishEventAsync(new PlayerInvitedEvent(command.GameId, command.Host, command.Player));

        return Result.Success();
    }
}