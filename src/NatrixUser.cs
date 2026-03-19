namespace NatrixServices;

public record NatrixUser(string UserId)
{
    public static NatrixUser GetUser(string userId)
    {
        return new(userId);
    }
}