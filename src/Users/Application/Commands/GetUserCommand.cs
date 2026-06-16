using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;
using NatrixServices.Users.Application.Interfaces;
using NatrixServices.Users.Core.Entities;

namespace NatrixServices.Users.Application.Commands;

public record GetUserCommand(string Username) : ICommand<UserData>;

public class GetUserCommandHandler(IUserStorage userStorage) : ICommandHandler<GetUserCommand, UserData>
{
    public async Task<Result<UserData>> HandleAsync(GetUserCommand command)
    {
        var user = await userStorage.GetUserAsync(command.Username);
        if (user == null) return new Error(ErrorType.NotFound, $"User with name \"{command.Username}\" not found!");

        return user;
    }
}