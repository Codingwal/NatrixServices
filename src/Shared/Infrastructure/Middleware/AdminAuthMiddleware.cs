using NatrixServices.Shared.Application;

namespace NatrixServices.Shared.Infrastructure.Middleware;

public class AdminAuthMiddleware : IMiddleware
{
    private static string AdminPassword { get; } = "admin"; // TODO 

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.GetEndpoint();
        var authAttr = endpoint?.Metadata.GetMetadata<AuthAsAdminAttribute>();

        if (authAttr != null)
        {
            string? password = context.Request.Headers["password"];

            if (password == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Admin password is required");
                return;
            }

            if (password != AdminPassword)  // is the password correct?
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Wrong admin password");
                return;
            }
        }

        await next(context);
    }
}