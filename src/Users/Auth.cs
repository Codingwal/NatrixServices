using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NatrixServices.Users;

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

    public static async Task<bool> AuthenticateUserAsync(HttpRequest request, UserDataContext dataContext)
    {
        string? username = request.Headers["username"];
        string? passwordHash = request.Headers["passwordHash"];

        if (username == null || passwordHash == null)
            return false;

        return await dataContext.AuthenticateAsync(username, passwordHash);
    }
}

public class AdminOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        bool allowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(m => m is AllowAnonymousAttribute);
        if (allowAnonymous) return;

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
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        IServiceProvider serviceProvider = context.HttpContext.RequestServices;
        var dataContext = serviceProvider.GetRequiredService<UserDataContext>();

        if (!await Auth.AuthenticateUserAsync(context.HttpContext.Request, dataContext))
            context.Result = new UnauthorizedObjectResult("Invalid username or password hash");

        await base.OnActionExecutionAsync(context, next);
    }
}