using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace NatrixServices.Shared.Infrastructure.Database;

public static class ItemStorageUtility
{
    public static async Task<bool> EntityExistsAsync<TEntity>(DbContext context, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return await context.Set<TEntity>().AnyAsync(predicate);
    }
    public static async Task<bool> EntityExistsAsync<TEntity, TId>(DbContext context, TId id)
        where TEntity : class
        where TId : notnull
    {
        var entityType = context.Model.FindEntityType(typeof(TEntity))
            ?? throw new InvalidOperationException($"The type {typeof(TEntity).Name} is not managed by this db context");

        var primaryKeyName = entityType.FindPrimaryKey()?.Properties[0].Name
            ?? throw new InvalidOperationException($"No primary key has been defined for entity {typeof(TEntity).Name}");

        return await EntityExistsAsync<TEntity>(context, e => EF.Property<TId>(e, primaryKeyName).Equals(id));
    }

    public static async Task<TEntity?> GetEntityOrDefaultAsync<TEntity, TId>(DbContext context, TId id)
        where TEntity : class
    {
        return await context.Set<TEntity>().FindAsync(id);
    }

    public static async Task<TEntity> GetEntityAsync<TEntity, TId>(DbContext context, TId id)
        where TEntity : class
    {
        return await GetEntityOrDefaultAsync<TEntity, TId>(context, id)
            ?? throw new KeyNotFoundException($"Entity with id {id} does not exist.");
    }

    public static async Task<IEnumerable<TEntity>> GetEntitiesAsync<TEntity>(DbContext context, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return await context.Set<TEntity>().Where(predicate).ToListAsync();
    }

    public static async Task AddEntityAsync<TEntity, TId>(DbContext context, TEntity entity, TId id)
        where TEntity : class
        where TId : notnull
    {
#if DEBUG
        if (await EntityExistsAsync<TEntity, TId>(context, id))
            throw new InvalidOperationException($"An entity with the same id {id} has already been added");
#endif

        context.Set<TEntity>().Add(entity);
        await context.SaveChangesAsync();
    }

    public static async Task UpdateEntityAsync<TEntity, TId>(DbContext context, TEntity entity, Func<TEntity, TId> idSelector)
        where TEntity : class
        where TId : notnull
    {
        var set = context.Set<TEntity>();
        TId id = idSelector(entity);

#if DEBUG
        if (!await EntityExistsAsync<TEntity, TId>(context, id))
            throw new KeyNotFoundException($"Cannot update {entity} as it has not been added to the db. Did you mean to use AddEntity?");
#endif

        var trackedEntity = context.ChangeTracker.Entries<TEntity>()
            .FirstOrDefault(e => id.Equals(idSelector(e.Entity)));

        if (trackedEntity == null)
            set.Update(entity);
        else if (trackedEntity.Entity != entity)
            trackedEntity.CurrentValues.SetValues(entity);

        await context.SaveChangesAsync();
    }

    public static async Task DeleteEntityAsync<TEntity, TId>(DbContext context, TId id)
        where TEntity : class
        where TId : notnull
    {
#if DEBUG
        if (!await EntityExistsAsync<TEntity, TId>(context, id))
            throw new KeyNotFoundException($"Entity with id {id} could not be found and therefor cannot be deleted.");
#endif

        var entityType = context.Model.FindEntityType(typeof(TEntity))
            ?? throw new InvalidOperationException($"The type {typeof(TEntity).Name} is not managed by this db context.");

        var primaryKeyName = entityType.FindPrimaryKey()?.Properties[0].Name
            ?? throw new InvalidOperationException($"No primary key has been defined for entity {typeof(TEntity).Name}.");

        await context.Set<TEntity>()
            .Where(e => EF.Property<TId>(e, primaryKeyName).Equals(id))
            .ExecuteDeleteAsync();
    }
}