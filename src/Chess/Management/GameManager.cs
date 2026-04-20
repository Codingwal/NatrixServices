using NatrixServices.Chess.Core;
using NatrixServices.Chess.Data;

namespace NatrixServices.Chess.Management;

public class GameNotFoundException() : Exception("Game not found");
public class GameFullException() : Exception("Game is full");
public class GameFinishedException() : Exception("Game is already finished");
public class GameNotStartedException() : Exception("Still waiting for players");
public class NotParticipantException() : Exception("You are not a participant of this game");
public class InvalidMoveException(string message) : Exception(message);

public interface IGameManager
{
    Task<GameData> GetGameDataAsync(GameId gameId);
    Task<List<GameData>> GetGamesAsync(bool onlyPublic, string? filter = null, string? username = null);
    Task<List<Move>> GetAllowedMovesAsync(GameId gameId, string? field = null);
    Task<GameId> CreateGameAsync(string name, bool isPublic, int timePerPlayer);
    Task JoinGameAsync(string username, GameId gameId);
    Task DoMoveAsync(GameId gameId, Move move, string username);
}

public class GameManager(IItemStorage<GameData, GameId> DataContext) : IGameManager
{
    public async Task<GameData> GetGameDataAsync(GameId gameId)
    {
        GameData? gameData = await DataContext.GetItemAsync(gameId) ?? throw new GameNotFoundException();
        return gameData;
    }

    public async Task<List<GameData>> GetGamesAsync(bool onlyPublic, string? filter = null, string? username = null)
    {
        List<GameData> games = await DataContext.GetAllItemsAsync();

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

    public async Task<List<Move>> GetAllowedMovesAsync(GameId gameId, string? field = null)
    {
        GameData? gameData = await DataContext.GetItemAsync(gameId) ?? throw new GameNotFoundException();

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
        await DataContext.AddItemAsync(gameData);
        return gameId;
    }

    public async Task JoinGameAsync(string username, GameId gameId)
    {
        GameData? gameData = await DataContext.GetItemAsync(gameId) ?? throw new GameNotFoundException();

        if (gameData.Player1 == null)
            gameData.Player1 = username;
        else if (gameData.Player2 == null)
            gameData.Player2 = username;
        else
            throw new GameFullException();

        await DataContext.SaveChangesAsync();
    }

    public async Task DoMoveAsync(GameId gameId, Move move, string username)
    {
        GameData? gameData = await DataContext.GetItemAsync(gameId) ?? throw new GameNotFoundException();

        if (gameData.Result != null)
            throw new GameFinishedException();

        if (gameData.Player1 == null || gameData.Player2 == null)
            throw new GameNotStartedException();

        // Get player index
        int playerIndex;
        if (gameData.Player1 == username)
            playerIndex = 1;
        else if (gameData.Player2 == username)
            playerIndex = 2;
        else
            throw new NotParticipantException();

        // Load the game from FEN
        ChessGame game = new(gameData.Fen);

        // Check move using the chess engine
        ChessEngine engine = new(game);
        string? error = engine.CheckMove(move);
        if (error != null)
            throw new InvalidMoveException(error);

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
        await DataContext.SaveChangesAsync();
    }
}