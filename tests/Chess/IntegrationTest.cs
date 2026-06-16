using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NatrixServices.Chess.Application.Commands;
using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;

namespace NatrixServices.Test.Chess;

public class CommandIntegrationTests
{
    private IServiceProvider ServiceProvider { get; }
    public CommandIntegrationTests()
    {
        var services = new ServiceCollection();

        services.AddScoped<IGameStorage, MockGameStorage>();
        services.AddScoped<IUserStorage, MockUserStorage>();

        services.AddScoped<ICommandHandler<CreateGameCommand, GameId>, CreateGameCommandHandler>();
        services.AddScoped<ICommandHandler<JoinGameCommand>, JoinGameCommandHandler>();

        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();

        ServiceProvider = services.BuildServiceProvider();
    }

    private const string gameName = "My fancy game";
    private const bool isPublic = true;
    private readonly TimeSpan timePerPlayer = TimeSpan.FromMinutes(42);
    private const string playerName = "Mario";

    [Fact]
    public async Task TestCreateGame()
    {
        var dispatcher = ServiceProvider.GetRequiredService<ICommandDispatcher>();
        var gameStorage = ServiceProvider.GetRequiredService<IGameStorage>();

        var createGameCommand = new CreateGameCommand(gameName, isPublic, timePerPlayer);

        Assert.True((await dispatcher.ExecuteCommandAsync<CreateGameCommand, GameId>(createGameCommand)).TryGetValue(out GameId gameId, out var error),
            $"{error}");

        var game = await gameStorage.GetGameAsync(gameId);
        Assert.NotNull(game);

        Assert.Equal(gameId, game.GameId);
        Assert.Equal(gameName, game.Name);
        Assert.Equal(isPublic, game.IsPublic);
        Assert.Equal(timePerPlayer, game.TimePerPlayer);

        var joinGameCommand = new JoinGameCommand(gameId, playerName);

        Assert.False((await dispatcher.ExecuteCommandAsync(joinGameCommand)).TryGetError(out error),
            $"{error}");

        game = await gameStorage.GetGameAsync(gameId);
        Assert.NotNull(game);
        Assert.Equal(playerName, game.PlayerWhite);
    }
}