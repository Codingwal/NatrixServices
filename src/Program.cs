using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

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

        builder.Services.AddDbContext<DnsBlocker.DataContext>(options => options.UseSqlite("Data Source=data/dnsblocker/data.db"));
        builder.Services.AddHostedService<DnsBlocker.DnsBlockerService>();


        /* Setup chess storage */

        builder.Services.AddDbContext<Chess.Data.GameDataContext>(options => options.UseSqlite("Data Source=data/chess/games.db"));
        builder.Services.AddScoped<IItemStorage<Chess.Data.GameData, GameId>>(sp => sp.GetRequiredService<Chess.Data.GameDataContext>());

        builder.Services.AddDbContext<Chess.Data.UserDataContext>(options => options.UseSqlite("Data Source=data/chess/users.db"));
        builder.Services.AddScoped<IItemStorage<Chess.Data.UserData, GameId>>(sp => sp.GetRequiredService<Chess.Data.UserDataContext>());

        builder.Services.AddDbContext<Chess.Data.EventDataContext>(options => options.UseSqlite("Data Source=data/chess/events.db"));
        builder.Services.AddScoped<IItemStorage<Chess.Data.EventData, GameId>>(sp => sp.GetRequiredService<Chess.Data.EventDataContext>());


        /* Setup chess managers*/

        builder.Services.AddScoped<Chess.Management.IGameManager, Chess.Management.GameManager>();
        builder.Services.AddScoped<Chess.Management.IEventManager, Chess.Management.EventManager>();


        builder.Services.AddDbContext<Users.UserDataContext>(options => options.UseSqlite("Data Source=data/users/data.db"));
        builder.Services.AddScoped<IItemStorage<Users.UserData, string>>(sp => sp.GetRequiredService<Users.UserDataContext>());


        WebApplication app = builder.Build();

        app.UseCors();

        app.UseDefaultFiles();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseStaticFiles();

        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()!.Error;

                context.Response.StatusCode = (int)ExceptionMapper.GetStatusCode(exception);

                await context.Response.WriteAsJsonAsync(new
                {
                    error = (context.Response.StatusCode != 500) ? exception.Message : "Internal Server Error"
                });
            });
        });

        // Setup databases
        Directory.CreateDirectory("data");
        using (var scope = app.Services.CreateScope())
        {
            Directory.CreateDirectory("data/dnsblocker");
            scope.ServiceProvider.GetRequiredService<DnsBlocker.DataContext>().Init();

            Directory.CreateDirectory("data/chess");
            await scope.ServiceProvider.GetRequiredService<Chess.Data.GameDataContext>().InitAsync();
            await scope.ServiceProvider.GetRequiredService<Chess.Data.UserDataContext>().InitAsync();

            Directory.CreateDirectory("data/users");
            await scope.ServiceProvider.GetRequiredService<Users.UserDataContext>().InitAsync();
        }

        app.MapControllers();

        Console.WriteLine("Starting WebApplication");
        app.Run();
    }
}
