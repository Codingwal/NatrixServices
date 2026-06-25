using NatrixServices.Chess.Application.Events;
using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;

namespace NatrixServices.Chess.Application.EventHandlers;

public class UpdateXP(IGameStorage gameStorage, IUserStorage userStorage) : IEventHandler<GameFinishedEvent>
{
    private const int xpWin = 20;
    private const int xpLose = 3;
    private const int xpDraw = 10;

    public async Task HandleAsync(GameFinishedEvent e)
    {
        ChessGame game = await gameStorage.GetGameAsync(e.GameId)
            ?? throw new KeyNotFoundException($"Could not find game with gameId {e.GameId}");

        UserData userWhite = await userStorage.GetUserAsync(game.PlayerWhite!)
            ?? throw new KeyNotFoundException($"Could not find user with username \"{game.PlayerWhite}\"");

        UserData userBlack = await userStorage.GetUserAsync(game.PlayerBlack!)
            ?? throw new KeyNotFoundException($"Could not find user with username \"{game.PlayerBlack}\"");

        if (!game.MatchResult.HasValue)
            throw new InvalidOperationException($"Expected game to be finished");

        userWhite.XP += CalculateXPChange(Players.White, game.MatchResult.Value);
        await userStorage.UpdateUserAsync(userWhite);

        userBlack.XP += CalculateXPChange(Players.Black, game.MatchResult.Value);
        await userStorage.UpdateUserAsync(userBlack);
    }

    private static int CalculateXPChange(Players player, GameResult gameResult)
    {
        if (gameResult == GameResult.Draw)
            return xpDraw;

        else if (player == Players.White && gameResult == GameResult.WinWhite
                || player == Players.Black && gameResult == GameResult.WinBlack)
            return xpWin;
        else
            return xpLose;
    }
}