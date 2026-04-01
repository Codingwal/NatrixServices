using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices;

public static class Program
{
    public static void Main(string[] args)
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

        builder.Services.AddDbContext<DnsBlocker.ConfigContext>(options => options.UseSqlite("Data Source=data/dnsblocker/config.db"));
        builder.Services.AddDbContext<DnsBlocker.DataContext>(options => options.UseSqlite("Data Source=data/dnsblocker/data.db"));
        builder.Services.AddHostedService<DnsBlocker.DnsBlockerService>();

        builder.Services.AddDbContext<Chess.DataContext>(options => options.UseSqlite("Data Source=data/chess/data.db"));

        builder.Services.AddDbContext<Users.DataContext>(options => options.UseSqlite("Data Source=data/users/data.db"));

        WebApplication app = builder.Build();

        app.UseCors();

        app.UseDefaultFiles();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseStaticFiles();

        // Setup databases
        Directory.CreateDirectory("data");
        using (var scope = app.Services.CreateScope())
        {
            Directory.CreateDirectory("data/dnsblocker");
            scope.ServiceProvider.GetRequiredService<DnsBlocker.ConfigContext>().Init();
            scope.ServiceProvider.GetRequiredService<DnsBlocker.DataContext>().Init();

            Directory.CreateDirectory("data/chess");
            scope.ServiceProvider.GetRequiredService<Chess.DataContext>().Init();

            Directory.CreateDirectory("data/users");
            scope.ServiceProvider.GetRequiredService<Users.DataContext>().Init();
        }

        app.MapControllers();

        Console.WriteLine("Starting WebApplication");
        app.Run();
    }
}
