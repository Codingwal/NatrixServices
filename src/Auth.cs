using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NatrixServices;

public static class Auth
{
    public static string? VerifyAdminPassword(HttpRequest request)
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

    public static bool AuthenticateUser(HttpRequest request, Users.DataContext dataContext)
    {
        string? username = request.Headers["username"];
        string? passwordHash = request.Headers["passwordHash"];

        if (username == null || passwordHash == null)
            return false;

        return dataContext.Authenticate(username, passwordHash);
    }
}

public class AdminOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        string? error = Auth.VerifyAdminPassword(context.HttpContext.Request);

        if (error == null)
            return;

        context.Result = new ContentResult()
        {
            Content = error,
            StatusCode = 401
        };
    }
}

public class HeaderAuthAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        IServiceProvider serviceProvider = context.HttpContext.RequestServices;
        Users.DataContext dataContext = serviceProvider.GetRequiredService<Users.DataContext>();

        if (!Auth.AuthenticateUser(context.HttpContext.Request, dataContext))
            context.Result = new UnauthorizedResult();
    }
}