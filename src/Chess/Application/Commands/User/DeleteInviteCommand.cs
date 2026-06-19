using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Application.Commands;

public record DeleteInviteCommand(GameId GameId, string Player) : ICommand;

public class DeleteInviteCommandHandler(IUserStorage userStorage) : ICommandHandler<DeleteInviteCommand>
{
    public async Task<Result> HandleAsync(DeleteInviteCommand command)
    {
        var user = await userStorage.GetUserAsync(command.Player);
        if (user == null) return new Error(ErrorType.NotFound, $"User with username \"{command.Player}\" not found.");

        int removedCount = user.Invites.RemoveAll(invite => invite.GameId == command.GameId);

        if (removedCount == 0)
            return new Error(ErrorType.NotFound, $"Invite with gameId {command.GameId} not found (could not be deleted)");

        await userStorage.UpdateUserAsync(user);

        return Result.Success();
    }
}