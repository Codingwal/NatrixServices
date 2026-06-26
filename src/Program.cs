using Microsoft.AspNetCore.HttpOverrides;
using NatrixServices.Betting;
using NatrixServices.Betting.Infrastructure;
using NatrixServices.Chess;
using NatrixServices.Chess.Infrastructure;
using NatrixServices.Shared;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Infrastructure.Middleware;
using NatrixServices.Users;
using NatrixServices.Users.Infrastructure;

namespace NatrixServices;

public static class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        builder.WebHost.UseUrls("http://0.0.0.0:5000");

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod() // allow PATCH, DELETE, ...
                    .AllowAnyHeader();
            });
        });

        builder.Services.AddControllers();


        // -- Setup application -- //

        builder.Services.AddUserServices();
        builder.Services.AddChessServices();
        builder.Services.AddBettingServices();

        builder.Services.AddApplicationLayerServices(typeof(Program).Assembly);


        // -- Setup middleware -- //

        builder.Services.AddScoped<AdminAuthMiddleware>();
        builder.Services.AddScoped<AuthorizationMiddleware>();
        builder.Services.AddScoped<UserAuthMiddleware>();


        // -- Configure app -- //

        WebApplication app = builder.Build();

        app.UseCors();
        app.UseDefaultFiles();
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
        // app.UseStaticFiles();
        app.UseRouting();


        // -- Setup middleware -- //

        // Must come after UseRouting and before MapControllers
        app.UseMiddleware<UserAuthMiddleware>();
        app.UseMiddleware<AdminAuthMiddleware>();
        app.UseMiddleware<AuthorizationMiddleware>();


        // -- Init databases -- //

        Directory.CreateDirectory("data");
        using (var scope = app.Services.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<ChessStorage>().InitAsync();
            await scope.ServiceProvider.GetRequiredService<UserStorage>().InitAsync();
            await scope.ServiceProvider.GetRequiredService<BettingStorage>().InitAsync();
        }

        app.MapControllers();


        Console.WriteLine("Starting WebApplication");
        var appTask = app.RunAsync();

        // Must run after app.Run() 
        VerifyAllEndpointsHaveAuthAttribute(app);

        await appTask;
    }

    private static void VerifyAllEndpointsHaveAuthAttribute(WebApplication app)
    {
        var endpoints = app.Services.GetServices<EndpointDataSource>().SelectMany(src => src.Endpoints);

        List<string> badEndpoints = [];

        foreach (var endpoint in endpoints)
        {
            if (endpoint.Metadata.GetMetadata<AuthAttribute>() == null)
                badEndpoints.Add(endpoint.DisplayName ?? "Unknown Endpoint");
        }

        if (badEndpoints.Count != 0)
            throw new Exception($"The following endpoints are missing AuthAttribute: \n- {string.Join("\n- ", badEndpoints)}");
    }
}