using NatrixServices.Chess.Core.Entities;

namespace NatrixServices.Chess.Application.Interfaces;

public interface IGameStorage
{
    public Task SaveGameAsync(ChessGame game);
    public Task<ChessGame?> GetGameAsync(GameId gameId);
    public Task<IEnumerable<ChessGame>> GetAllGamesAsync(bool onlyPublic, GameStatus? status, string? player);
}

public interface IUserStorage
{
    public Task SaveUserAsync(UserData user);
    public Task<UserData> GetUserAsync(string username);
    public Task<UserData?> GetorCreateUserAsync(string username);
    public Task<IEnumerable<UserData>> GetAllUsersAsync();
}