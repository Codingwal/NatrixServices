using NatrixServices.Chess.Core;
using NatrixServices.Chess.Data;

namespace NatrixServices.Chess.Management;

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

    Task<GameData?> GetGameDataAsync(GameId gameId);
    Task<Result<string>> GetFenAsync(GameId gameId);
    Task<List<GameData>> GetGamesAsync(bool onlyPublic, string? filter = null, string? username = null);
    Task<Result<List<Move>>> GetAllowedMovesAsync(GameId gameId, string? field = null);
    Task<GameId> CreateGameAsync(string name, bool isPublic, int timePerPlayer);
    Task<Result> JoinGameAsync(string username, GameId gameId);
    Task<Result> DoMoveAsync(GameId gameId, Move move, string username);
}

public class GameManager(IItemStorage<GameData, GameId> GameStorage) : IGameManager
{
    public async Task<GameData?> GetGameDataAsync(GameId gameId)
    {
        return await GameStorage.GetItemOrDefaultAsync(gameId);
    }

    public async Task<Result<string>> GetFenAsync(GameId gameId)
    {
        var gameData = await GetGameDataAsync(gameId);
        if (gameData == null) return IGameManager.Errors.NotFound;
        return gameData.Fen;
    }

    public async Task<List<GameData>> GetGamesAsync(bool onlyPublic, string? filter = null, string? username = null)
    {
        List<GameData> games = await GameStorage.GetAllItemsAsync();

        if (onlyPublic)
            games = games.Where(g => g.IsPublic).ToList();

        bool WaitingForPlayers(GameData game) => game.Player1 == null || game.Player2 == null;
        bool IsDone(GameData game) => game.Result != null;

        if (filter == "active")
            games = games.Where(g => !WaitingForPlayers(g) && !IsDone(g)).ToList();
        else if (filter == "done")
            games = games.Where(IsDone).ToList();
        else if (filter == "scheduled")
            games = []; // TODO: Update if game scheduling is implemented
        else if (filter == "waiting")
            games = games.Where(WaitingForPlayers).ToList();
        else { } // Ignore invalid or null filters

        if (username != null)
            games = games.Where(g => g.Player1 == username || g.Player2 == username).ToList();

        return games;
    }

    public async Task<Result<List<Move>>> GetAllowedMovesAsync(GameId gameId, string? field = null)
    {
        var gameData = await GetGameDataAsync(gameId);
        if (gameData == null) return IGameManager.Errors.NotFound;

        ChessGame game = new(gameData.Fen);
        ChessEngine engine = new(game);

        List<Move> allowedMoves;
        if (field != null)
            allowedMoves = engine.GetAllowedMoves(ChessEngine.FieldDescToPos(field));
        else
            allowedMoves = engine.GetAllowedMoves();

        return allowedMoves;
    }

    public async Task<GameId> CreateGameAsync(string name, bool isPublic, int timePerPlayer)
    {
        GameId gameId = Utility.GenerateId();
        GameData gameData = new(gameId, name, isPublic, timePerPlayer, ChessGame.DefaultFen);
        await GameStorage.AddItemAsync(gameData);
        return gameId;
    }

    public async Task<Result> JoinGameAsync(string username, GameId gameId)
    {
        var gameData = await GetGameDataAsync(gameId);
        if (gameData == null) return IGameManager.Errors.NotFound;

        if (gameData.Player1 == username || gameData.Player2 == username)
            return IGameManager.Errors.AlreadyParticipant;

        if (gameData.Player1 == null)
            gameData.Player1 = username;
        else if (gameData.Player2 == null)
            gameData.Player2 = username;
        else
            return IGameManager.Errors.GameFull;

        await GameStorage.UpdateItemAsync(gameData);
        return Result.Success();
    }

    public async Task<Result> DoMoveAsync(GameId gameId, Move move, string username)
    {
        var gameData = await GetGameDataAsync(gameId);
        if (gameData == null) return IGameManager.Errors.NotFound;

        if (gameData.Result != null)
            return IGameManager.Errors.GameFinished;

        if (gameData.Player1 == null || gameData.Player2 == null)
            return IGameManager.Errors.GameNotStarted;

        // Get player index
        int playerIndex;
        if (gameData.Player1 == username)
            playerIndex = 1;
        else if (gameData.Player2 == username)
            playerIndex = 2;
        else
            return IGameManager.Errors.NotParticipant;

        // Load the game from FEN
        ChessGame game = new(gameData.Fen);

        // Check move using the chess engine
        ChessEngine engine = new(game);
        string? error = engine.CheckMove(move);
        if (error != null)
            return IGameManager.Errors.InvalidMove(error);

        // Do the move and check if the game is finished
        game.DoMove(move);
        char? result = engine.CheckResult();

        // Handle time
        if (gameData.LastMoveTime != default)
        {
            TimeSpan timeElapsed = DateTimeOffset.UtcNow - gameData.LastMoveTime;
            if (playerIndex == 1)
            {
                gameData.TimeLeft1 -= timeElapsed;
                if (gameData.TimeLeft1 <= TimeSpan.Zero)
                    gameData.Result = 'b'; // Player 1 loses
            }
            else
            {
                gameData.TimeLeft2 -= timeElapsed;
                if (gameData.TimeLeft2 <= TimeSpan.Zero)
                    gameData.Result = 'w'; // Player 2 loses
            }
        }

        // Update game data
        gameData.Result = result;
        gameData.LastMoveTime = DateTimeOffset.UtcNow;
        gameData.Moves.Add(move);
        gameData.Fen = game.ToFen();
        await GameStorage.UpdateItemAsync(gameData);

        return Result.Success();
    }
}