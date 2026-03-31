using Microsoft.EntityFrameworkCore;

namespace NatrixServices.DnsBlocker;

public class DataContext(DbContextOptions<DataContext> options) : DataContext<UserData, GlobalData>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // The StringList objects must be serialized as json
        modelBuilder.Entity<UserData>(b =>
        {
            b.OwnsOne(u => u.LastRequest);
        });

        modelBuilder.Entity<GlobalData>(b =>
        {
            b.OwnsOne(u => u.LastRequest);
        });
    }
}

public class UserData : UserDataBase
{
    public int DnsRequestCount { get; set; } = 0;
    public DnsRequest LastRequest { get; set; } = new();
}

public class GlobalData
{
    public int DnsRequestCount { get; set; } = 0;
    public DnsRequest LastRequest { get; set; } = new();
}

public record DnsRequest
{
    public DateTimeOffset Time { get; set; } = default;
    public string Domain { get; set; } = string.Empty;
    public bool Blocked { get; set; } = false;
    public UserId? UserId { get; set; } = null;
    public DeviceId? DeviceId { get; set; } = null;
}