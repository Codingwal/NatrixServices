global using DeviceId = string;
global using FilterId = string;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices.DnsBlocker;

public class ConfigContext(DbContextOptions<ConfigContext> options) : DataContext<UserConfig, GlobalConfig>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // The StringList objects must be serialized as json
        modelBuilder.Entity<UserConfig>(b =>
        {
            b.Property(u => u.Devices).SaveAsJson();
            b.Property(u => u.Filters).SaveAsJson();
        });

        modelBuilder.Entity<GlobalConfig>(b =>
        {
            b.Property(u => u.Filters).SaveAsJson();
        });
    }
}

public interface IBlockingConfig
{
    public bool EnableBlocking { get; set; }
}

public class UserConfig : UserDataBase, IBlockingConfig
{
    public bool EnableBlocking { get; set; } = false;
    public List<DeviceConfig> Devices { get; set; } = [];
    public List<FilterReference> Filters { get; set; } = [];
}
public class DeviceConfig : IBlockingConfig
{
    public DeviceId Device { get; set; } = DeviceId.Empty;
    public bool EnableBlocking { get; set; } = false;
}
public class FilterReference : IBlockingConfig
{
    public FilterId Filter { get; set; } = FilterId.Empty;
    public bool EnableBlocking { get; set; } = true;
}

public class GlobalConfig : IBlockingConfig
{
    public bool EnableBlocking { get; set; } = true;
    public bool EnableDnsServer { get; set; } = true;
    public Dictionary<FilterId, FilterConfig> Filters = [];
}
public class FilterConfig
{
    public FilterId Id { get; set; } = FilterId.Empty;
    public List<string> DomainsToBlock { get; set; } = [];
}