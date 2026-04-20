using Microsoft.EntityFrameworkCore;

namespace NatrixServices.Chess.Data;

public class GameDataContext(DbContextOptions<GameDataContext> options) : DatabaseItemStorage<GameData, GameId>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<GameData>(b =>
        {
            b.Property(g => g.Moves).SaveAsJson();
        });
    }
}

public class UserDataContext(DbContextOptions<UserDataContext> options) : DatabaseItemStorage<UserData, string>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserData>(b =>
        {
            b.Property(u => u.Stats).SaveAsJson();
        });
    }
}