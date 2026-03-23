using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NatrixServices;

public static class Auth
{
    public static string? VerifyPassword(HttpRequest request)
    {
        // Check if a password is present
        if (!request.Headers.TryGetValue("password", out var password))
        {
            return "Admin password required";
        }

        // Verify password
        if (password != Password)  // is the password correct?
        {
            return $"Wrong admin password \"password\"";
        }

        return null; // No error
    }
    public static string Password { get; } = "admin";
}

public class AdminOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        string? error = Auth.VerifyPassword(context.HttpContext.Request);

        if (error == null)
            return;

        context.Result = new ContentResult()
        {
            Content = error,
            StatusCode = 401
        };
    }
}