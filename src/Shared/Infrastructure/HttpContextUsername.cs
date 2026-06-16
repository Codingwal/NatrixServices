namespace NatrixServices.Shared.Infrastructure;

public static class HttpContextUsernameExtension
{
    public static void SetUsername(this HttpContext context, string username)
    {
        context.Items["username"] = username;
    }
    public static string GetUsername(this HttpContext context)
    {
        return context.Items["username"] as string ?? throw new Exception("Username not found in HttpContext");
    }
}