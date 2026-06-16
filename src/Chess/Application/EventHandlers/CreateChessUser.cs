using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Users.Application.Events;

namespace NatrixServices.Chess.Application.EventHandlers;

public class CreateChessUser(IUserStorage userStorage) : IEventHandler<CreateUserEvent>
{
    public async Task HandleAsync(CreateUserEvent e)
    {
        UserData user = new(e.Username);
        await userStorage.AddUserAsync(user);
    }
}