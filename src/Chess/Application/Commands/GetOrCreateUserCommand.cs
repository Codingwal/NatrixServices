using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Application.Commands;

public record GetOrCreateUserCommand(string Username) : ICommand<UserData>;

public class GetOrCreateUserCommandHandler(IUserStorage UserStorage) : ICommandHandler<GetOrCreateUserCommand, UserData>
{
    public async Task<Result<UserData>> HandleAsync(GetOrCreateUserCommand command)
    {
        var game = await UserStorage.GetUserAsync(command.Username);
        return game;
    }
}