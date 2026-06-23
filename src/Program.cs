using System.Reflection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using NatrixServices.Chess.Core.Engine;
using NatrixServices.Chess.Core.Services;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Infrastructure.Middleware;

namespace NatrixServices;

using ChessStorage = Chess.Infrastructure.ChessStorage;
using IChessGameStorage = Chess.Application.Interfaces.IGameStorage;
using IChessUserStorage = Chess.Application.Interfaces.IUserStorage;
using IChessEventStorage = Chess.Application.Interfaces.IEventStorage;

using UserStorage = Users.Infrastructure.UserStorage;
using IUserStorage = Users.Application.Interfaces.IUserStorage;

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


        // -- Setup database services -- //

        builder.Services.AddDbContext<ChessStorage>(options => options.UseSqlite($"Data Source=data/chess.db"));
        builder.Services.AddScoped<IChessGameStorage>(sp => sp.GetRequiredService<ChessStorage>());
        builder.Services.AddScoped<IChessUserStorage>(sp => sp.GetRequiredService<ChessStorage>());
        builder.Services.AddScoped<IChessEventStorage>(sp => sp.GetRequiredService<ChessStorage>());

        builder.Services.AddDbContext<UserStorage>(options => options.UseSqlite($"Data Source=data/users.db"));
        builder.Services.AddScoped<IUserStorage>(sp => sp.GetRequiredService<UserStorage>());


        // -- Setup application layer -- //

        AddCommandHandlers(builder.Services, typeof(Program).Assembly);
        AddEventHandlers(builder.Services, typeof(Program).Assembly);
        AddBackgroundTasks(builder.Services, typeof(Program).Assembly);

        builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        builder.Services.AddSingleton<IEventManager, EventManager>();
        builder.Services.AddHostedService<BackgroundTaskUpdater>();


        // -- Setup middleware -- //

        builder.Services.AddScoped<AdminAuthMiddleware>();
        builder.Services.AddScoped<AuthorizationMiddleware>();
        builder.Services.AddScoped<UserAuthMiddleware>();


        // -- Setup special services -- //

        builder.Services.AddSingleton<IChessEngine, ChessEngine>();


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
        }

        app.MapControllers();


        Console.WriteLine("Starting WebApplication");
        var appTask = app.RunAsync();

        // Must run after app.Run() 
        VerifyAllEndpointsHaveAuthAttribute(app);

        await appTask;
    }

    private static void AddCommandHandlers(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract);

        foreach (var type in types)
        {
            var handlerInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType
                    && (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)
                    || i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
                );

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.AddScoped(handlerInterface, type);
            }
        }
    }

    private static void AddEventHandlers(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract);

        foreach (var type in types)
        {
            var handlerInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)
                );

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.AddScoped(handlerInterface, type);
            }
        }
    }

    private static void AddBackgroundTasks(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => typeof(IBackgroundTask).IsAssignableFrom(t));

        foreach (var type in types)
        {
            services.AddTransient(type); // Register as concrete type so they can be obtained using GetRequiredService()
            services.AddTransient(typeof(IBackgroundTask), type); // Register as interface so they are found on startup
        }
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