using NatrixServices.Chess.Core.Entities;
using EventId = NatrixServices.Chess.Core.Entities.EventId;

namespace NatrixServices.Chess.Application.Interfaces;

public interface IGameStorage
{
    public Task AddGameAsync(ChessGame game);
    public Task AddGamesAsync(IEnumerable<ChessGame> games);
    public Task<ChessGame?> GetGameAsync(GameId gameId);
    public Task<IEnumerable<ChessGame>> GetAllGamesAsync(bool onlyPublic, GameStatus? status, string? player);
    public Task<IEnumerable<ChessGame>> GetAllNotDoneGamesAsync();
    public Task UpdateGameAsync(ChessGame game);
    public Task UpdateGamesAsync(IEnumerable<ChessGame> game);
}

public interface IUserStorage
{
    public Task AddUserAsync(UserData user);
    public Task<UserData?> GetUserAsync(string username);
    public Task<IEnumerable<UserData>> GetAllUsersAsync();
    public Task UpdateUserAsync(UserData user);
}

public interface IEventStorage
{
    public Task AddEventAsync(ChessEvent chessEvent);
    public Task<ChessEvent?> GetEventAsync(EventId eventId);
    public Task<IEnumerable<ChessEvent>> GetAllEventsAsync();
    public Task UpdateEventAsync(ChessEvent chessEvent);
}