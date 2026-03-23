using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices;

public abstract class UserDataBase
{
    [Key]
    [Required]
    [StringLength(8)]
    public string UserId { get; set; } = string.Empty;
}

public class DataContext<TUser, TGlobal> : DbContext
    where TUser : UserDataBase
    where TGlobal : class, new()
{
    public DbSet<TUser> UserData => Set<TUser>();
    public DbSet<TGlobal> GlobalData => Set<TGlobal>();

    public DataContext(DbContextOptions options) : base(options) { }

    public void Init()
    {
        Database.EnsureCreated();

        if (!GlobalData.Any())
        {
            TGlobal defaultGlobal = new();

            Entry(defaultGlobal).Property("id").CurrentValue = "default";

            GlobalData.Add(defaultGlobal);
            SaveChanges();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // SQLite needs some sort of id, TGlobal doesn't contain any as there is only one element
        modelBuilder.Entity<TGlobal>().Property<string>("id");
        modelBuilder.Entity<TGlobal>().HasKey("id");

        // Prevent shared table
        modelBuilder.Entity<TUser>().ToTable("UserData");
        modelBuilder.Entity<TGlobal>().ToTable("GlobalData");
    }

    public async Task<TUser?> GetUserData(UserId userId)
    {
        TUser? userData = await UserData.FindAsync(userId);
        return userData;
    }
    public async Task SetUserData(UserId userId, TUser data)
    {
        if (data.UserId != userId)
            throw new($"UserId mismatch! (\"{userId}\" != \"{data.UserId}\")");

        TUser? existingEntity = await UserData.FindAsync(userId);

        if (existingEntity == null)
        {
            UserData.Add(data);
        }
        else
        {
            Entry(existingEntity).CurrentValues.SetValues(data);
        }
        await SaveChangesAsync();
    }
    public async Task<TGlobal> GetGlobalData()
    {
        TGlobal? data = await GlobalData.FirstOrDefaultAsync();

        if (data == null)
            throw new($"Global data ({typeof(TGlobal)}) is not initialized");

        return data;
    }
    public async Task SetGlobalData(TGlobal data)
    {
        TGlobal? existing = await GlobalData.FirstOrDefaultAsync();

        if (existing == null)
            throw new($"Global data ({typeof(TGlobal)}) is not initialized");

        Entry(existing).CurrentValues.SetValues(data);

        await SaveChangesAsync();
    }
}