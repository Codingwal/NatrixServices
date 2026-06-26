using Microsoft.EntityFrameworkCore;
using NatrixServices.Betting.Application.Services;
using NatrixServices.Betting.Core.ProfitCalculator;
using NatrixServices.Betting.Core.Services;
using NatrixServices.Betting.Infrastructure;

namespace NatrixServices.Betting;

public static class SetupExtensions
{
    public static void AddBettingServices(this IServiceCollection services)
    {
        services.AddDbContext<BettingStorage>(options => options.UseSqlite($"Data Source=data/betting.db"));
        services.AddScoped<IUserStorage>(sp => sp.GetRequiredService<BettingStorage>());
        services.AddScoped<IMatchStorage>(sp => sp.GetRequiredService<BettingStorage>());
        services.AddScoped<IBetStorage>(sp => sp.GetRequiredService<BettingStorage>());

        services.AddSingleton<IProfitCalculator, ProfitCalculator>();
    }
}