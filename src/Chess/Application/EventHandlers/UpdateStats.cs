using NatrixServices.Chess.Application.Events;
using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;

namespace NatrixServices.Chess.Application.EventHandlers;

public class UpdateStats(IGameStorage gameStorage, IUserStorage userStorage) : IEventHandler<GameFinishedEvent>
{
    public async Task HandleAsync(GameFinishedEvent e)
    {
        ChessGame game = await gameStorage.GetGameAsync(e.GameId)
            ?? throw new KeyNotFoundException($"Could not find game with gameId {e.GameId}");

        UserData userWhite = await userStorage.GetUserAsync(game.PlayerWhite!)
            ?? throw new KeyNotFoundException($"Could not find user with username \"{game.PlayerWhite}\"");

        UserData userBlack = await userStorage.GetUserAsync(game.PlayerBlack!)
            ?? throw new KeyNotFoundException($"Could not find user with username \"{game.PlayerBlack}\"");

        userWhite.Stats.UpdateStats(game, Players.White);
        await userStorage.UpdateUserAsync(userWhite);

        userBlack.Stats.UpdateStats(game, Players.Black);
        await userStorage.UpdateUserAsync(userBlack);
    }
}