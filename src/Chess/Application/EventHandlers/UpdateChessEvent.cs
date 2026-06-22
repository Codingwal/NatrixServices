using NatrixServices.Chess.Application.Events;
using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;

namespace NatrixServices.Chess.Application.EventHandlers;

public class UpdateChessEvent(IGameStorage gameStorage) : IEventHandler<GameFinishedEvent>
{
    public async Task HandleAsync(GameFinishedEvent e)
    {
        ChessGame? game = await gameStorage.GetGameAsync(e.GameId) ?? throw new KeyNotFoundException();

        if (!game.EventId.HasValue)
            return;

        // ...
    }
}