using Microsoft.EntityFrameworkCore;
using NatrixServices.Betting.Application.Services;
using NatrixServices.Betting.Core.Entities;
using NatrixServices.Shared.Infrastructure.Database;

namespace NatrixServices.Betting.Infrastructure;

public class BettingStorage(DbContextOptions<BettingStorage> options) : DbContext(options), IUserStorage, IMatchStorage, IBetStorage
{
    private DbSet<UserData> Users => Set<UserData>();
    private DbSet<Match> Matches => Set<Match>();
    private DbSet<Bet> Bets => Set<Bet>();

    public async Task InitAsync() => await Database.EnsureCreatedAsync();


    // User methods

    public async Task AddUserAsync(UserData user)
        => await ItemStorageUtility.AddEntityAsync(this, user, user.Username);
    public async Task<UserData?> GetUserAsync(string username)
        => await Users.FindAsync(username);

    public async Task<IEnumerable<UserData>> GetAllUsersAsync()
        => await Users.ToListAsync();

    public async Task UpdateUserAsync(UserData user)
        => await ItemStorageUtility.UpdateEntityAsync(this, user, u => u.Username);

    public async Task UpdateUsersAsync(IEnumerable<UserData> users)
        => await ItemStorageUtility.UpdateEntitiesAsync(this, users, u => u.Username);


    // Match methods

    public async Task AddMatchAsync(Match match)
        => await ItemStorageUtility.AddEntityAsync(this, match, match.MatchId);

    public async Task<Match?> GetMatchAsync(MatchId matchId)
        => await Matches.FindAsync(matchId);

    public async Task<IEnumerable<Match>> GetAllMatchesAsync()
        => await Matches.ToListAsync();

    public async Task<IEnumerable<Match>> GetMatchesAsync(bool? open, string? Event)
    {
        var time = DateTime.UtcNow;

        return await Matches
            .Where(m => open == null || open == (time < m.StartTime))
            .Where(m => Event == null || m.Event == Event)
            .ToListAsync();
    }

    public async Task UpdateMatchAsync(Match match)
        => await ItemStorageUtility.UpdateEntityAsync(this, match, m => m.MatchId);


    // Bet methods

    public async Task AddBetAsync(Bet bet)
    {
        Bets.Add(bet);
        await SaveChangesAsync();
    }

    public async Task<IEnumerable<Bet>> GetBetsAsync(string? owner = null, MatchId? matchId = null, bool? open = null)
    {
        return await Bets
            .Where(b => owner == null || b.Owner == owner)
            .Where(b => open == null || b.Done != open)
            .ToListAsync();
    }

    public async Task UpdateBetAsync(Bet bet)
    {
        Bets.Update(bet);
        await SaveChangesAsync();
    }

    public async Task UpdateBetsAsync(IEnumerable<Bet> bets)
    {
        Bets.UpdateRange(bets);
        await SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserData>(builder =>
        {
            builder.HasKey(u => u.Username);
        });

        modelBuilder.Entity<Match>(builder =>
        {
            builder.HasKey(m => m.MatchId);
            builder.Property(m => m.MatchId).HasConversion(m => m.Value, str => new MatchId(str));

            builder.OwnsOne(m => m.Odds);
            builder.OwnsOne(m => m.Result);
        });

        modelBuilder.Entity<Bet>(builder =>
        {
            builder.HasKey(b => new { b.Owner, b.MatchId });

            builder.Property(b => b.MatchId).HasConversion(m => m.Value, str => new MatchId(str));

            builder.OwnsOne(b => b.ExpectedResult);
        });
    }
}