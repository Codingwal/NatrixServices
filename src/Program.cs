using Microsoft.EntityFrameworkCore;

namespace NatrixServices;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        builder.Services.AddControllers();

        builder.Services.AddDbContext<DnsBlocker.ConfigContext>(options => options.UseSqlite("Data Source=data/dnsblocker/config.db"));
        builder.Services.AddDbContext<DnsBlocker.DataContext>(options => options.UseSqlite("Data Source=data/dnsblocker/data.db"));
        builder.Services.AddHostedService<DnsBlocker.DnsBlocker>();

        WebApplication app = builder.Build();

        // Setup databases
        Directory.CreateDirectory("data");
        using (var scope = app.Services.CreateScope())
        {
            Directory.CreateDirectory("data/dnsblocker");
            scope.ServiceProvider.GetRequiredService<DnsBlocker.ConfigContext>().Init();
            scope.ServiceProvider.GetRequiredService<DnsBlocker.DataContext>().Init();
        }

        app.MapControllers();

        Console.WriteLine("Starting WebApplication");
        app.Run();
    }
}
