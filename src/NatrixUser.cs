namespace NatrixServices;

public record NatrixUser(string UserId, string Username);

public static class UserManager
{
    public static NatrixUser GetUser(string userId)
    {

    }

    public static NatrixUser CreateUser(string username)
    {
        string userId = Guid.NewGuid().ToString();

        return new NatrixUser(userId, username);
    }
}