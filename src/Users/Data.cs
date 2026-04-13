using Microsoft.EntityFrameworkCore;

namespace NatrixServices.Users;

public class DataContext(DbContextOptions<DataContext> options) : DataContext<UserData, GlobalData>(options)
{
    public async Task<bool> UserExists(string username) => await UserData.AnyAsync(u => u.Username == username);
    public bool Authenticate(string username, string passwordHash)
    {
        UserData? userData = UserData.Find(username);
        if (userData == null) return false;

        return userData.PasswordHash == passwordHash;
    }
}

public class UserData : UserDataBase
{
    public string PasswordHash { get; set; } = string.Empty;
    public string LinkedAccount { get; set; } = string.Empty; // Not used yet
}
public class GlobalData
{

}