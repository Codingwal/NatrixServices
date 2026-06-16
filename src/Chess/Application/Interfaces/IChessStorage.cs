using NatrixServices.Chess.Core.Entities;

namespace NatrixServices.Chess.Application.Interfaces;

public interface IGameStorage
{
    public Task AddGameAsync(ChessGame game);
    public Task<ChessGame?> GetGameAsync(GameId gameId);
    public Task<IEnumerable<ChessGame>> GetAllGamesAsync(bool onlyPublic, GameStatus? status, string? player);
    public Task UpdateGameAsync(ChessGame game);
}

public interface IUserStorage
{
    public Task AddUserAsync(UserData user);
    public Task<UserData?> GetUserAsync(string username);
    public Task<IEnumerable<UserData>> GetAllUsersAsync();
    public Task UpdateUserAsync(UserData user);
}