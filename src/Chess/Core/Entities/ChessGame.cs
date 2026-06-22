using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Core.Entities;

public enum Players { White, Black }
public enum GameStatus { WaitingForPlayers, Waiting, Active, Done }
public enum GameResult { WinWhite, WinBlack, Draw }
public record Move(Int2 Origin, Int2 Destination, ChessFigure? Promotion = null);
public record DrawOffer(string Player);
public record ChessGame(GameId GameId, string Name, bool IsPublic, TimeSpan TimePerPlayer, string Fen, EventId? EventId = null)
{
    public GameStatus Status { get; private set; } = GameStatus.WaitingForPlayers;

    public string Fen { get; private set; } = Fen;
    public List<Move> Moves { get; } = [];
    public GameResult? MatchResult { get; private set; } = null;
    public Players NextPlayer { get; private set; } = Players.White;

    public string? PlayerWhite { get; private set; } = null;
    public string? PlayerBlack { get; private set; } = null;

    public DrawOffer? DrawOffer { get; private set; } = null;

    public TimeSpan TimeLeftWhite { get; private set; } = TimePerPlayer;
    public TimeSpan TimeLeftBlack { get; private set; } = TimePerPlayer;
    public DateTime? StartTime { get; private set; } = null;
    public DateTime LastMoveTime { get; private set; } = default;
    private DateTime lastClockUpdateTime = default;

    public Result JoinGame(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
            return Result.Failure(ErrorType.BadRequest, "Player name cannot be null or empty!");

        if (Status != GameStatus.WaitingForPlayers)
            return Result.Failure(ErrorType.Conflict, "Game is already full!");

        if (GetPlayer(playerName) != null)
            return Result.Failure(ErrorType.BadRequest, $"Player \"{PlayerWhite}\" cannot play against themselves!");

        if (PlayerWhite == null)
            PlayerWhite = playerName;
        else
            PlayerBlack = playerName;

        if (PlayerWhite != null && PlayerBlack != null)
            Status = GameStatus.Waiting;

        return Result.Success();
    }

    public Result StartGame()
    {
        if (Status != GameStatus.Waiting)
            return Result.Failure(ErrorType.Conflict, "Game cannot be started! The game must be in waiting state.");

        StartTime = DateTime.UtcNow;
        Status = GameStatus.Active;
        LastMoveTime = DateTime.UtcNow;
        lastClockUpdateTime = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Schedule(DateTime startTime)
    {
        if (Status != GameStatus.Waiting)
            return new Error(ErrorType.Conflict, "Game can only be scheduled if in waiting state.");

        if (StartTime != null)
            return new Error(ErrorType.Conflict, "Game has already been started or scheduled.");

        if (startTime < DateTime.UtcNow)
            return new Error(ErrorType.BadRequest, "Requested start time has already passed. Use StartGame().");

        StartTime = startTime;

        return Result.Success();
    }

    public Result DoMove(string newFen, GameResult? result, string playerName)
    {
        if (Status != GameStatus.Active)
            return Result.Failure(ErrorType.Conflict, "Game is not active!");

        Players? player = GetPlayer(playerName);
        if (player == null)
            return new Error(ErrorType.Forbidden, $"Player \"{player}\" is not a participant of this game.");

        if (player != NextPlayer)
            return new Error(ErrorType.Forbidden, "It's not your turn!");

        Fen = newFen;

        // Update result and status
        if (result != null)
        {
            MatchResult = result;
            Status = GameStatus.Done;
        }

        // Must be before updating NextPlayer
        if (UpdateTime().TryGetError(out var error))
            return error;

        LastMoveTime = DateTime.UtcNow;
        NextPlayer = (NextPlayer == Players.White) ? Players.Black : Players.White;

        return Result.Success();
    }

    public Result OfferDraw(string playerName)
    {
        if (Status != GameStatus.Active)
            return Result.Failure(ErrorType.Conflict, "Game is not active!");

        if (GetPlayer(playerName) == null)
            return new Error(ErrorType.Forbidden, $"Player \"{playerName}\" is not a participant of this game.");

        if (DrawOffer == null)
            DrawOffer = new DrawOffer(playerName);
        else
        {
            if (DrawOffer.Player != playerName)
            {
                MatchResult = GameResult.Draw;
                Status = GameStatus.Done;
            }
            else
                return Result.Failure(ErrorType.Conflict, $"Player \"{playerName}\" is already offering a draw.");
        }

        return Result.Success();
    }
    public Result DeclineDraw(string playerName)
    {
        if (Status != GameStatus.Active)
            return Result.Failure(ErrorType.Conflict, "Game is not active!");

        if (GetPlayer(playerName) == null)
            return new Error(ErrorType.Forbidden, $"Player \"{playerName}\" is not a participant of this game.");

        if (DrawOffer == null)
            return new Error(ErrorType.NotFound, $"There currently is no draw offer in game {GameId}.");

        DrawOffer = null;

        return Result.Success();
    }

    public Result Resign(string playerName)
    {
        if (Status != GameStatus.Active)
            return Result.Failure(ErrorType.Conflict, "Game is not active!");

        Players? player = GetPlayer(playerName);
        if (player == null)
            return new Error(ErrorType.Forbidden, $"Player \"{playerName}\" is not a participant of this game.");

        MatchResult = (player == Players.White) ? GameResult.WinBlack : GameResult.WinWhite;
        Status = GameStatus.Done;

        return Result.Success();
    }

    private Result UpdateTime()
    {
        if (Status != GameStatus.Active)
            return Result.Failure(ErrorType.Conflict, "Game is not active!");

        if (NextPlayer == Players.White)
        {
            TimeLeftWhite -= DateTime.UtcNow - lastClockUpdateTime;

            if (TimeLeftWhite <= TimeSpan.Zero)
            {
                TimeLeftWhite = TimeSpan.Zero;
                MatchResult = GameResult.WinBlack;
                Status = GameStatus.Done;
            }
        }
        else
        {
            TimeLeftBlack -= DateTime.UtcNow - lastClockUpdateTime;

            if (TimeLeftBlack <= TimeSpan.Zero)
            {
                TimeLeftBlack = TimeSpan.Zero;
                MatchResult = GameResult.WinWhite;
                Status = GameStatus.Done;
            }
        }

        lastClockUpdateTime = DateTime.UtcNow;

        return Result.Success();
    }

    public Players? GetPlayer(string playerName)
    {
        if (playerName == PlayerWhite)
            return Players.White;
        else if (playerName == PlayerBlack)
            return Players.Black;
        else
            return null;
    }

    public Result Update()
    {
        if (Status == GameStatus.Waiting)
        {
            if (StartTime != null && StartTime < DateTime.UtcNow)
                StartGame();
        }

        if (Status == GameStatus.Active)
            if (UpdateTime().TryGetError(out var error)) return error;

        return Result.Success();
    }
}