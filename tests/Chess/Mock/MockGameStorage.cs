using System.Collections.Concurrent;
using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;

namespace NatrixServices.Test.Chess;

public class MockGameStorage : IGameStorage
{
    private ConcurrentDictionary<GameId, ChessGame> Games { get; } = new();
    public async Task AddGameAsync(ChessGame game)
    {
        Games[game.GameId] = game;
    }

    public async Task<IEnumerable<ChessGame>> GetAllGamesAsync(bool onlyPublic, GameStatus? status, string? player)
    {
        return Games.Values;
    }

    public async Task<ChessGame?> GetGameAsync(GameId gameId)
    {
        return Games[gameId];
    }

    public async Task UpdateGameAsync(ChessGame game)
    {
        Games[game.GameId] = game;
    }

    public void LogGames()
    {
        foreach (var (gameId, game) in Games)
            Console.WriteLine($"{gameId}: \"{game.Name}\" | {game.PlayerWhite} vs {game.PlayerBlack} | {game.TimePerPlayer}");
    }
}