using Microsoft.EntityFrameworkCore;
using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Infrastructure.Database;

namespace NatrixServices.Chess.Infrastructure;

public class ChessStorage(DbContextOptions options) : DbContext(options), IUserStorage, IGameStorage
{
    private DbSet<UserData> Users => Set<UserData>();
    private DbSet<ChessGame> Games => Set<ChessGame>();

    public async Task InitAsync() => await Database.EnsureCreatedAsync();


    // User methods

    public async Task AddUserAsync(UserData user)
        => await ItemStorageUtility.AddEntityAsync(this, user, user.Username);

    public async Task<IEnumerable<UserData>> GetAllUsersAsync()
        => await Users.ToListAsync();

    public async Task<UserData?> GetUserAsync(string username)
        => await Users.FindAsync(username);

    public async Task UpdateUserAsync(UserData user)
        => await ItemStorageUtility.UpdateEntityAsync(this, user, u => u.Username);


    // Game methods

    public async Task AddGameAsync(ChessGame game)
        => await ItemStorageUtility.AddEntityAsync(this, game, game.GameId);

    public async Task<IEnumerable<ChessGame>> GetAllGamesAsync(bool onlyPublic, GameStatus? status, string? player)
    {
        return await Games.Where(g =>
            (!onlyPublic || g.IsPublic)
            && (status == null || g.Status == status)
            && (player == null || g.PlayerWhite == player || g.PlayerBlack == player))
            .ToListAsync();
    }

    public async Task<ChessGame?> GetGameAsync(GameId gameId)
        => await Games.FindAsync(gameId);

    public async Task UpdateGameAsync(ChessGame game)
        => await ItemStorageUtility.UpdateEntityAsync(this, game, g => g.GameId);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserData>(builder =>
        {
            builder.HasKey(u => u.Username);

            builder.Property(u => u.Stats).SaveAsJson();
        });

        modelBuilder.Entity<ChessGame>(builder =>
        {
            builder.HasKey(g => g.GameId);

            builder.Property(g => g.GameId).HasConversion(id => id.Value, str => new GameId(str));

            builder.Property(g => g.Status).HasConversion<string>();

            builder.Property(g => g.Moves).SaveAsJson();
            builder.Property(g => g.MatchResult).HasConversion<string>();
            builder.Property(g => g.NextPlayer).HasConversion<string>();

            builder.Property<DateTime>("lastClockUpdateTime");
        });
    }
}