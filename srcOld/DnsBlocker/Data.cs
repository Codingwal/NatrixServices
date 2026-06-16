global using DeviceId = string;
global using FilterId = string;

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
            b.Property(u => u.Devices).SaveAsJson();
            b.Property(u => u.Filters).SaveAsJson();
        });

        modelBuilder.Entity<GlobalData>(b =>
        {
            b.OwnsOne(u => u.LastRequest);
            b.Property(u => u.Filters).SaveAsJson();
        });
    }
}

public interface IBlockingConfig
{
    public bool EnableBlocking { get; set; }
}

public class UserData : UserDataBase, IBlockingConfig
{
    public int DnsRequestCount { get; set; } = 0;
    public DnsRequestDTO LastRequest { get; set; } = new();
    public bool EnableBlocking { get; set; } = false;
    public Dictionary<DeviceId, DeviceConfigDTO> Devices { get; set; } = [];
    public Dictionary<FilterId, FilterReferenceDTO> Filters { get; set; } = [];
}

public class GlobalData : IBlockingConfig
{
    public int DnsRequestCount { get; set; } = 0;
    public DnsRequestDTO LastRequest { get; set; } = new();
    public bool EnableBlocking { get; set; } = true;
    public bool EnableDnsServer { get; set; } = true;
    public Dictionary<FilterId, FilterConfigDTO> Filters = [];
}