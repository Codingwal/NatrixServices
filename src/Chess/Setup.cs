using Microsoft.EntityFrameworkCore;
using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Engine;
using NatrixServices.Chess.Core.MatchGenerator;
using NatrixServices.Chess.Core.Services;
using NatrixServices.Chess.Infrastructure;

namespace NatrixServices.Chess;

public static class SetupExtensions
{
    public static void AddChessServices(this IServiceCollection services)
    {
        services.AddDbContext<ChessStorage>(options => options.UseSqlite($"Data Source=data/chess.db"));
        services.AddScoped<IGameStorage>(sp => sp.GetRequiredService<ChessStorage>());
        services.AddScoped<IUserStorage>(sp => sp.GetRequiredService<ChessStorage>());
        services.AddScoped<IEventStorage>(sp => sp.GetRequiredService<ChessStorage>());

        services.AddSingleton<IChessEngine, ChessEngine>();
        services.AddSingleton<IMatchGenerator, MatchGenerator>();
    }
}