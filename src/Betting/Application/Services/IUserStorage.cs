using NatrixServices.Betting.Core.Entities;

namespace NatrixServices.Betting.Application.Services;

public interface IUserStorage
{
    public Task AddUserAsync(UserData user);
    public Task<UserData?> GetUserAsync(string username);
    public Task<IEnumerable<UserData>> GetAllUsersAsync();
    public Task UpdateUserAsync(UserData user);
    public Task UpdateUsersAsync(IEnumerable<UserData> users);
}