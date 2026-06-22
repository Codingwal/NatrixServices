using NatrixServices.Chess.Application.Events;
using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;

namespace NatrixServices.Chess.Application.BackgroundTasks;

public class UpdateGamesTask(IGameStorage gameStorage, IEventManager eventManager) : IBackgroundTask
{
    public TimeSpan UpdateInterval => TimeSpan.FromSeconds(3);

    public async Task ExecuteAsync()
    {
        var games = await gameStorage.GetAllNotDoneGamesAsync();

        List<ChessGame> gamesToUpdate = [];
        List<IEvent> eventsToPublish = [];
        foreach (ChessGame game in games)
        {
            game.Update();
            gamesToUpdate.Add(game);

            if (game.MatchResult != null)
                eventsToPublish.Add(new GameFinishedEvent(game.GameId));
        }

        await gameStorage.UpdateGamesAsync(gamesToUpdate);

        await Task.WhenAll(eventsToPublish.Select(e => eventManager.PublishEventAsync(e)));
    }
}