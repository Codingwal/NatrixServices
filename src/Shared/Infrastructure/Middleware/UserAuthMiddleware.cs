namespace NatrixServices.Shared.Infrastructure.Middleware;

public class UserAuthMiddleware(UserDataContext UserDataContext) : IMiddleware
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
                await context.Response.WriteAsync("Username and password hash are required");
                return;
            }

            if (!await UserDataContext.AuthenticateAsync(username, passwordHash))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid username or password");
                return;
            }

            context.SetUsername(username);
        }

        await next(context);
    }
}