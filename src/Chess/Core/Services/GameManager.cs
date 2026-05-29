namespace NatrixServices.Chess.Core;

public interface IGameManager
{
    public class Errors
    {
        public static readonly Error NotFound = new(ErrorType.NotFound, "Game not found");
        public static readonly Error GameFull = new(ErrorType.Conflict, "Game is full");
        public static readonly Error GameFinished = new(ErrorType.Gone, "Game is already finished");
        public static readonly Error GameNotStarted = new(ErrorType.Conflict, "Still waiting for players");
        public static readonly Error NotParticipant = new(ErrorType.Forbidden, "You are not a participant of this game");
        public static readonly Error AlreadyParticipant = new(ErrorType.Conflict, "You are already a participant of this game");
        public static Error InvalidMove(string message) => new(ErrorType.Unprocessable, message);
    }

    Task<GameInfo?> GetGameInfoAsync(GameId gameId);
    Task<Result<string>> GetFenAsync(GameId gameId);
    Task<List<GameInfo>> GetGamesAsync(bool onlyPublic, string? filter = null, string? username = null);
    Task<Result<List<Move>>> GetAllowedMovesAsync(GameId gameId, string? field = null);
    Task<GameId> CreateGameAsync(string name, bool isPublic, int timePerPlayer);
    Task<Result> JoinGameAsync(string username, GameId gameId);
    Task<Result> DoMoveAsync(GameId gameId, Move move, string username);
}