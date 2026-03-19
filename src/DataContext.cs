using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices;

public class DataContext : DbContext
{
    public DbSet<UserData> UserDatas => Set<UserData>();
    public DbSet<DnsBlockerConfig> DnsBlockerConfigs => Set<DnsBlockerConfig>();

    public DataContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DnsBlockerConfig>().OwnsOne(
            config => config.DomainsToBlock,
            b => b.ToJson()
        );
    }

    public async Task AddOrUpdate<T>(DbSet<T> dbSet, UserId userId, T data)
        where T : class
    {
        T? existingEntity = await dbSet.FindAsync(userId);

        if (existingEntity == null)
        {
            dbSet.Add(data);
        }
        else
        {
            Entry(existingEntity).CurrentValues.SetValues(data);
        }
    }
}

public class UserData
{
    [Key] public UserId UserId { get; set; } = UserId.Empty;

    public string Name { get; set; } = string.Empty;
}

public class DnsBlockerConfig
{
    [Key] public UserId UserId { get; set; } = UserId.Empty;

    public bool Block { get; set; } = false;
    public string IPAddress { get; set; } = string.Empty;
    public DomainList DomainsToBlock { get; set; } = new();
}

public class DomainList
{
    public List<string> Domains { get; set; } = new();
}