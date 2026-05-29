using NatrixServices.Chess.Core;
using NatrixServices.Chess.Data;

namespace NatrixServices.Chess.Management;

public class GameManager(IItemStorage<GameData, GameId> GameStorage) : IGameManager
{
    public async Task<GameInfo?> GetGameInfoAsync(GameId gameId)
    {
        var game = await GetGameDataAsync(gameId);
        return game?.ToGameInfo();
    }

    public async Task<Result<string>> GetFenAsync(GameId gameId)
    {
        var game = await GetGameDataAsync(gameId);
        if (game == null) return IGameManager.Errors.NotFound;
        return game.Fen;
    }

    public async Task<List<GameInfo>> GetGamesAsync(bool onlyPublic, string? filter = null, string? username = null)
    {
        List<GameInfo> games = (await GameStorage.GetAllItemsAsync()).Select(g => g.ToGameInfo()).ToList();

        if (onlyPublic)
            games = games.Where(g => g.IsPublic).ToList();

        if (filter == "active")
            games = games.Where(g => g.Active).ToList();
        else if (filter == "done")
            games = games.Where(g => g.Done).ToList();
        else if (filter == "scheduled")
            games = games.Where(g => g.Scheduled).ToList();
        else if (filter == "waiting")
            games = games.Where(g => g.Waiting).ToList();
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
        GameInfo gameInfo = GameInfo.CreateGameInfo(gameId, name, isPublic, timePerPlayer, ChessGame.DefaultFen);
        await GameStorage.AddItemAsync(GameData.FromGameInfo(gameInfo));
        return gameId;
    }

    public async Task<Result> JoinGameAsync(string username, GameId gameId)
    {
        var game = await GetGameDataAsync(gameId);
        if (game == null) return IGameManager.Errors.NotFound;

        if (game.Player1 == username || game.Player2 == username)
            return IGameManager.Errors.AlreadyParticipant;

        if (game.Player1 == null)
            game.Player1 = username;
        else if (game.Player2 == null)
            game.Player2 = username;
        else
            return IGameManager.Errors.GameFull;

        await GameStorage.UpdateItemAsync(game);
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

    private async Task<GameData?> GetGameDataAsync(GameId gameId)
    {
        return await GameStorage.GetItemOrDefaultAsync(gameId);
    }
}