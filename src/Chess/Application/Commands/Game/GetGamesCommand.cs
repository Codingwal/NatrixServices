using NatrixServices.Chess.Application.Interfaces;
using NatrixServices.Chess.Core.Entities;
using NatrixServices.Shared.Application;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Application.Commands;

using GameList = IEnumerable<ChessGame>;

public record GetGamesCommand(bool OnlyPublic, GameStatus? Status = null, string? Username = null)
    : ICommand<GameList>;

public class GetGamesCommandHandler(IGameStorage GameStorage) : ICommandHandler<GetGamesCommand, GameList>
{
    public async Task<Result<GameList>> HandleAsync(GetGamesCommand command)
    {
        GameList games = await GameStorage.GetAllGamesAsync(command.OnlyPublic, command.Status, command.Username);

        return Result<GameList>.Success(games);
    }
}