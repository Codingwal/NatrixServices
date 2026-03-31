using Microsoft.EntityFrameworkCore;

namespace NatrixServices.Users;

public class DataContext(DbContextOptions<DataContext> options) : DataContext<UserData, GlobalData>(options)
{
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
}
public class GlobalData
{

}