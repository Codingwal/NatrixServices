using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Application.Commands;

public record CreateGameCommand(string Name, bool IsPublic, TimeSpan TimePerPlayer) : ICommand<GameId>;

public class CreateGameCommandHandler(IGameStorage GameStorage) : ICommandHandler<CreateGameCommand, GameId>
{
    public async Task<Result<GameId>> HandleAsync(CreateGameCommand command)
    {
        var gameId = GameId.Generate();

        var game = new ChessGame(gameId, command.Name, command.IsPublic, command.TimePerPlayer, ChessGame.DefaultFen);

        await GameStorage.SaveGameAsync(game);

        return gameId;
    }
}