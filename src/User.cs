global using UserId = string;

namespace NatrixServices;

public static class User
{
    public static UserId Create()
    {
        string userId = Guid.NewGuid().ToString()[0..8];
        return userId;
    }
}