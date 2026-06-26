using NatrixServices.Betting.Application.Services;
using NatrixServices.Betting.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Betting.Application.Commands;

public record GetUserCommand(string Username) : ICommand<UserData>;

public class GetUserCommandHandler(IUserStorage userStorage) : ICommandHandler<GetUserCommand, UserData>
{
    public async Task<Result<UserData>> HandleAsync(GetUserCommand command)
    {
        var user = await userStorage.GetUserAsync(command.Username);
        if (user == null) return new Error(ErrorType.NotFound, $"User with username \"{command.Username}\" not found.");

        return user;
    }
}