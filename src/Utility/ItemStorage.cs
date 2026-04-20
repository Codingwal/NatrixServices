using Microsoft.EntityFrameworkCore;

namespace NatrixServices;

public interface IIdentifiable<TId> where TId : notnull
{
    TId Id { get; set; }
}

public interface IItemStorage<TItem, TId>
    where TItem : IIdentifiable<TId>
    where TId : notnull
{
    Task InitAsync();
    Task SaveChangesAsync();

    Task<bool> ItemExistsAsync(TId id);

    Task<TItem> GetItemAsync(TId id);
    Task<TItem> GetOrCreateItemAsync(TId id);
    Task<List<TItem>> GetAllItemsAsync();

    Task AddItemAsync(TItem item);
    Task DeleteItemAsync(TId id);
}

public class DatabaseItemStorage<TItem, TId>(DbContextOptions options) : DbContext(options), IItemStorage<TItem, TId>
    where TItem : class, IIdentifiable<TId>, new()
    where TId : notnull
{
    private DbSet<TItem> Items => Set<TItem>();

    public async Task InitAsync() => await Database.EnsureCreatedAsync();
    public async Task SaveChangesAsync() => await base.SaveChangesAsync();

    public async Task<bool> ItemExistsAsync(TId id) => await Items.AnyAsync(i => id.Equals(i.Id));

    public async Task<TItem> GetItemAsync(TId id)
    {
        var item = await Items.FindAsync(id) ?? throw new KeyNotFoundException($"Item with id {id} not found.");
        return item;
    }

    public async Task<TItem> GetOrCreateItemAsync(TId id)
    {
        var item = await Items.FindAsync(id);
        if (item == null)
        {
            item = new TItem { Id = id };
            await Items.AddAsync(item);
            await SaveChangesAsync();
        }
        return item;
    }

    public async Task<List<TItem>> GetAllItemsAsync() => await Items.ToListAsync();

    public async Task AddItemAsync(TItem item)
    {
        await Items.AddAsync(item);
        await SaveChangesAsync();
    }

    public async Task DeleteItemAsync(TId id)
    {
        var item = await Items.FindAsync(id) ?? throw new KeyNotFoundException($"Item with id {id} not found.");
        Items.Remove(item);
        await SaveChangesAsync();
    }
}