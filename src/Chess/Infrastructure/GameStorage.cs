using Microsoft.EntityFrameworkCore;
using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;

namespace NatrixServices.Chess.Infrastructure;

public class GameStorage(DbContextOptions options) : DbContext(options), IGameStorage
{
    private DbSet<ChessGame> Games => Set<ChessGame>();
    public async Task InitAsync() => await Database.EnsureCreatedAsync();
    public async Task<IEnumerable<ChessGame>> GetAllGamesAsync(bool onlyPublic, GameStatus? status, string? player)
    {
        return await Games.Where(g =>
            (!onlyPublic || g.IsPublic)
            && (status == null || g.Status == status)
            && (player == null || g.PlayerWhite == player || g.PlayerBlack == player))
            .ToListAsync();
    }

    public async Task<ChessGame?> GetGameAsync(GameId gameId)
    {
        return await Games.FindAsync(gameId);
    }

    public async Task SaveGameAsync(ChessGame game)
    {
        Games.Update(game);
        await SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ChessGame>(builder =>
        {
            builder.HasKey(g => g.GameId);

            builder.Property(g => g.GameId).HasConversion(id => id.Value, str => new GameId(str));

            builder.Property(g => g.Status).HasConversion<string>(); // Save as string for better readability

            builder.Property(g => g.Moves).SaveAsJson();
            builder.Property(g => g.MatchResult).HasConversion<string>();
            builder.Property(g => g.NextPlayer).HasConversion<string>();

            builder.Property<DateTime>("lastClockUpdateTime");
        });
    }
}