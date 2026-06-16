namespace NatrixServices.Shared.Infrastructure.Middleware;

public class AuthorizationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.GetEndpoint();
        var authAttr = endpoint?.Metadata.GetMetadata<AuthAsUserAttribute>();

        if (authAttr != null && authAttr.UsernameRouteParam != null)
        {
            var usernameAuth = context.GetUsername();

            if (!context.GetRouteData().Values.TryGetValue(authAttr.UsernameRouteParam, out var usernameRoute))
                throw new InvalidOperationException($"Route parameter \"{authAttr.UsernameRouteParam}\" not found");

            if (usernameAuth != usernameRoute as string)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("You are not authorized to access this resource");
                return;
            }
        }

        await next(context);
    }
}