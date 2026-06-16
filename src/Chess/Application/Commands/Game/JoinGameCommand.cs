using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Application.Commands;

public record JoinGameCommand(GameId GameId, string PlayerName) : ICommand;

public class JoinGameCommandHandler(IGameStorage GameStorage) : ICommandHandler<JoinGameCommand>
{
    public async Task<Result> HandleAsync(JoinGameCommand command)
    {
        var game = await GameStorage.GetGameAsync(command.GameId);
        if (game == null) return new Error(ErrorType.NotFound, $"Game with id {command.GameId} not found!");

        var result = game.AddPlayer(command.PlayerName);
        if (result.IsFailure) return result.Error;

        await GameStorage.UpdateGameAsync(game);

        return Result.Success();
    }
}