using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices;

public abstract class UserDataBase
{
    [Key]
    [Required]
    [StringLength(8)]
    public string Username { get; set; } = string.Empty;
}

public class DataContext<TUser, TGlobal>(DbContextOptions options) : DbContext(options)
    where TUser : UserDataBase, new()
    where TGlobal : class, new()
{
    public DbSet<TUser> UserData => Set<TUser>();
    public DbSet<TGlobal> GlobalData => Set<TGlobal>();

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
        // SQLite needs some sort of id. TGlobal doesn't contain any as there is only one element
        modelBuilder.Entity<TGlobal>().Property<string>("id");
        modelBuilder.Entity<TGlobal>().HasKey("id");

        // Prevent shared table
        modelBuilder.Entity<TUser>().ToTable("UserData");
        modelBuilder.Entity<TGlobal>().ToTable("GlobalData");
    }

    public async Task<TUser> GetUserData(string username)
    {
        TUser? userData = await UserData.FindAsync(username);

        if (userData == null)
        {
            userData = new TUser
            {
                Username = username
            };
            await UserData.AddAsync(userData);
        }

        return userData;
    }
    public async Task<TGlobal> GetGlobalData()
    {
        TGlobal? data = await GlobalData.SingleOrDefaultAsync()
            ?? throw new($"Global data ({typeof(TGlobal)}) is not initialized");
        return data;
    }
}