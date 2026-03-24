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
    [Required] public bool EnableBlocking { get; set; } = false;
    public Dictionary<DeviceId, DeviceConfig> Devices { get; set; } = [];
    public Dictionary<FilterId, FilterReference> Filters { get; set; } = [];
}
public class DeviceConfig : IBlockingConfig
{
    [Required] public DeviceId Id { get; set; } = DeviceId.Empty;
    [Required] public bool EnableBlocking { get; set; } = false;
}
public class FilterReference : IBlockingConfig
{
    [Required] public FilterId Id { get; set; } = FilterId.Empty;
    [Required] public bool EnableBlocking { get; set; } = true;
}

public class GlobalConfig : IBlockingConfig
{
    [Required] public bool EnableBlocking { get; set; } = true;
    [Required] public bool EnableDnsServer { get; set; } = true;
    public Dictionary<FilterId, FilterConfig> Filters = [];
}
public class FilterConfig
{
    [Required] public FilterId Id { get; set; } = FilterId.Empty;
    public string Description { get; set; } = string.Empty;
    [Required] public List<string> DomainsToBlock { get; set; } = [];
}