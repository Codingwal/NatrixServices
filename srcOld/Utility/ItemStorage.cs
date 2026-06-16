using Microsoft.EntityFrameworkCore;

namespace NatrixServices;


public class DatabaseItemStorage<TItem, TId>(DbContextOptions options) : DbContext(options)
    where TItem : class
{
    private DbSet<TItem> Items => Set<TItem>();

    protected async Task InitAsync() => await Database.EnsureCreatedAsync();

    protected async Task<bool> ItemExistsAsync(TId id) => GetItemOrDefaultAsync(id) != default;

    protected async Task<TItem?> GetItemOrDefaultAsync(TId id)
    {
        return await Items.FindAsync(id);
    }

    protected async Task<TItem> GetItemAsync(TId id)
    {
        return await GetItemOrDefaultAsync(id) ?? throw new KeyNotFoundException($"Item with id {id} not found.");
    }

    protected async Task<List<TItem>> GetAllItemsAsync() => await Items.ToListAsync();

    protected async Task AddItemAsync(TItem item)
    {
        await Items.AddAsync(item);
        await SaveChangesAsync();
    }

    protected async Task DeleteItemAsync(TId id)
    {
        var item = await Items.FindAsync(id) ?? throw new KeyNotFoundException($"Item with id {id} not found.");
        Items.Remove(item);
        await SaveChangesAsync();
    }

    protected async Task UpdateItemAsync(TItem item)
    {
        Items.Update(item);
        await SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TItem>();
    }
}