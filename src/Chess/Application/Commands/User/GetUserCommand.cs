using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Application.Commands;

public record GetUserCommand(string Username) : ICommand<UserData>;

public class GetUserCommandHandler(IUserStorage UserStorage) : ICommandHandler<GetUserCommand, UserData>
{
    public async Task<Result<UserData>> HandleAsync(GetUserCommand command)
    {
        var game = await UserStorage.GetUserAsync(command.Username);
        if (game == null) return new Error(ErrorType.NotFound, $"User with username \"{command.Username}\" not found.");

        return game;
    }
}