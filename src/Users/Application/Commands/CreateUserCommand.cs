using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;
using NatrixServices.Users.Application.Interfaces;
using NatrixServices.Users.Core.Entities;

namespace NatrixServices.Users.Application.Commands;

public record CreateUserCommand(string Username, string PasswordHash) : ICommand;

public class CreateUserCommandHandler(IUserStorage userStorage) : ICommandHandler<CreateUserCommand>
{
    private const int maxUsernameLength = 20;
    public async Task<Result> HandleAsync(CreateUserCommand command)
    {
        if (command.Username.Length >= maxUsernameLength)
            return new Error(ErrorType.BadRequest, $"Username \"{command.Username}\" is too long (max length: {maxUsernameLength})");

        if (!StandardChars.IsAllowed(command.Username))
            return new Error(ErrorType.BadRequest, $"Username \"{command.Username}\" contains forbidden characters.");

        if (await userStorage.UserExists(command.Username))
            return new Error(ErrorType.Conflict, $"User with username \"{command.Username}\" already exists.");

        UserData user = new(command.Username, command.PasswordHash);

        await userStorage.AddUserAsync(user);

        return Result.Success();
    }
}