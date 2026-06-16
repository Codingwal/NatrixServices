using NatrixServices.Users.Core.Entities;

namespace NatrixServices.Users.Application.Interfaces;

public interface IUserStorage
{
    public Task AddUserAsync(UserData user);
    public Task UpdateUserAsync(UserData user);
    public Task<UserData?> GetUserAsync(string username);
    public Task<IEnumerable<UserData>> GetAllUsersAsync();
    public Task<bool> UserExists(string username);
}