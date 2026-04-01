namespace NatrixServices;

public static class Utility
{
    public static string GenerateId()
    {
        string id = Guid.NewGuid().ToString()[0..8];
        return id;
    }
}