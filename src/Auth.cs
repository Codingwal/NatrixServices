namespace NatrixServices;

public static class Auth
{
    public static bool VerifyPassword(string password)
    {
        return password.Equals("admin", StringComparison.CurrentCultureIgnoreCase);
    }
}