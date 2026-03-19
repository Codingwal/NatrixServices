namespace NatrixServices;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers();

        WebApplication app = builder.Build();
        app.MapControllers();

        DnsBlocker dnsBlocker = new();
        dnsBlocker.Start();

        Console.WriteLine("Starting WebApplication");
        app.RunAsync();

        while (true) ;
    }
}
