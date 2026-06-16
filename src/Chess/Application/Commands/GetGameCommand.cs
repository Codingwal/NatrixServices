using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Application.Commands;

public record GetGameCommand(GameId GameId) : ICommand<ChessGame>;

public class GetGameCommandHandler(IGameStorage GameStorage) : ICommandHandler<GetGameCommand, ChessGame>
{
    public async Task<Result<ChessGame>> HandleAsync(GetGameCommand command)
    {
        var game = await GameStorage.GetGameAsync(command.GameId);
        if (game == null) return new Error(ErrorType.NotFound, $"Game with id {command.GameId} not found!");

        return game;
    }
}