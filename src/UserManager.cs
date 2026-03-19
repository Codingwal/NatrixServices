global using UserId = string;

namespace NatrixServices;

public static class UserManager
{
    public static async Task<string> CreateUserAsync(string username, DataContext dataContext)
    {
        string userId = Guid.NewGuid().ToString()[0..8];
        UserData userData = new() { UserId = userId, Name = username };

        dataContext.UserDatas.Add(userData);
        await dataContext.SaveChangesAsync();

        return userId;
    }

    public static async Task<UserData?> GetUserDataAsync(UserId userId, DataContext dataContext)
    {
        UserData? userData = await dataContext.UserDatas.FindAsync(userId);

        return userData;
    }
}