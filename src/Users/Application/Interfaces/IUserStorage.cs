namespace NatrixServices.Users.Application.Interfaces;

public interface IUserStorage
{
    public Task SaveUserAsync(UserData user);
    public Task<UserData?> GetUserAsync(string username);
    public Task<IEnumerable<UserData>> GetAllUsersAsync();
}