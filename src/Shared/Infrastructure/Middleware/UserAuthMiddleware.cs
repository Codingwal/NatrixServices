using NatrixServices.Shared.Application;
using NatrixServices.Users.Application.Interfaces;
using NatrixServices.Users.Core.Entities;

namespace NatrixServices.Shared.Infrastructure.Middleware;

public class UserAuthMiddleware(IUserStorage userStorage) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.GetEndpoint();
        var authAttr = endpoint?.Metadata.GetMetadata<AuthAsUserAttribute>();

        if (authAttr != null)
        {
            string? username = context.Request.Headers["username"];
            string? passwordHash = context.Request.Headers["passwordHash"];

            if (username == null || passwordHash == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { message = "Username and password hash are required" });
                return;
            }

            UserData? user = await userStorage.GetUserAsync(username);
            if (user == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { message = "User not found" });
                return;
            }

            bool validLogin = user.PasswordHash == passwordHash;

            if (!validLogin)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid username or password" });
                return;
            }

            context.SetUsername(username);
        }

        await next(context);
    }
}