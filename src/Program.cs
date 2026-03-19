using Microsoft.EntityFrameworkCore;

namespace NatrixServices;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        builder.Services.AddControllers();
        builder.Services.AddDbContext<DataContext>(options => options.UseSqlite("Data Source=multi_service.db"));
        builder.Services.AddHostedService<DnsBlocker>();

        WebApplication app = builder.Build();

        // Create db if it doesnt exist yet
        using (var scope = app.Services.CreateScope())
        {
            DataContext db = scope.ServiceProvider.GetRequiredService<DataContext>();
            db.Database.EnsureCreated();
        }

        app.MapControllers();

        Console.WriteLine("Starting WebApplication");
        app.RunAsync();

        while (true) ;
    }
}
