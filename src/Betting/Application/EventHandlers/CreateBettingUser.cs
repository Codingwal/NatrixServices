using NatrixServices.Betting.Application.Services;
using NatrixServices.Betting.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Users.Application.Events;

namespace NatrixServices.Betting.Application.EventHandlers;

public class CreateBettingUser(IUserStorage userStorage) : IEventHandler<CreateUserEvent>
{
    public async Task HandleAsync(CreateUserEvent e)
    {
        var user = new UserData(e.Username);
        await userStorage.AddUserAsync(user);
    }
}