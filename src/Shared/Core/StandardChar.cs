namespace NatrixServices.Shared.Core;

public static class StandardChars
{
    public static bool IsAllowed(char c)
    {
        return char.IsLetterOrDigit(c) || c == '-' || c == '_';
    }

    public static bool IsAllowed(string str)
    {
        return !str.Any(c => !IsAllowed(c));
    }
}