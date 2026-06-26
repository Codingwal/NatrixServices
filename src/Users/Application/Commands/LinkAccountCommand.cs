using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;
using NatrixServices.Users.Application.Interfaces;
using NatrixServices.Users.Core.Entities;

namespace NatrixServices.Users.Application.Commands;

public record LinkAccountCommand(string Username, string Account) : ICommand;

public class LinkAccountCommandHandler(IUserStorage userStorage) : ICommandHandler<LinkAccountCommand>
{
    public async Task<Result> HandleAsync(LinkAccountCommand command)
    {
        UserData? userData = await userStorage.GetUserAsync(command.Username);
        if (userData == null) return new Error(ErrorType.NotFound, $"User with name \"{command.Username}\" not found!");

        if (userData.LinkedAccount != null)
            return new Error(ErrorType.Conflict, $"There already is an account linked to user \"{command.Username}\"");

        userData.LinkedAccount = command.Account;

        return Result.Success();
    }
}