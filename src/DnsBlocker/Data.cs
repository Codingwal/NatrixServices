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
            b.Property(u => u.IPAddresses).SaveAsJson();
            b.Property(u => u.DomainsToBlock).SaveAsJson();
        });
    }

}

public class DataContext(DbContextOptions<DataContext> options) : DataContext<UserData, GlobalData>(options)
{

}

public class UserConfig : UserDataBase
{
    [Required] public bool EnableBlocking { get; set; } = false;
    public List<string> IPAddresses { get; set; } = [];
    public List<string> DomainsToBlock { get; set; } = [];
}

public class GlobalConfig
{
    [Required] public bool EnableBlocking { get; set; } = true;
    [Required] public bool EnableDnsServer { get; set; } = true;
}

public class UserData : UserDataBase
{
    public int DnsRequestCount { get; set; } = 0;
    public DateTimeOffset LastRequestTime { get; set; } = default;
}
public class GlobalData
{
    public int DnsRequestCount { get; set; } = 0;
    public DateTimeOffset LastRequestTime { get; set; } = default;
}