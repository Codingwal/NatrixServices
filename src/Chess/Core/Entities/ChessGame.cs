using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Core.Entities;

public enum Players { White, Black }
public enum GameStatus { WaitingForPlayers, Waiting, Active, Done }
public enum GameResult { WinWhite, WinBlack, Draw }
public record Move(Int2 Origin, Int2 Destination, ChessFigure? Promotion = null);
public record DrawOffer(string Player);
public class ChessGame(GameId gameId, string name, bool isPublic, TimeSpan timePerPlayer, string fen, EventId? eventId = null)
{
    public GameId GameId { get; } = gameId;

    public string Name { get; private set; } = name;
    public bool IsPublic { get; private set; } = isPublic;
    public GameStatus Status { get; private set; } = GameStatus.WaitingForPlayers;
    public EventId? EventId { get; private set; } = eventId;

    public string Fen { get; private set; } = fen;
    public List<Move> Moves { get; set; } = [];
    public GameResult? MatchResult { get; private set; } = null;
    public Players NextPlayer { get; private set; } = Players.White;

    public string? PlayerWhite { get; private set; } = null;
    public string? PlayerBlack { get; private set; } = null;

    public DrawOffer? DrawOffer { get; private set; } = null;

    public TimeSpan TimePerPlayer { get; private set; } = timePerPlayer;
    public TimeSpan TimeLeftWhite { get; private set; } = timePerPlayer;
    public TimeSpan TimeLeftBlack { get; private set; } = timePerPlayer;
    public DateTime StartTime { get; private set; } = default;
    public DateTime LastMoveTime { get; private set; } = default;
    private DateTime lastClockUpdateTime = default;

    public Result AddPlayer(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
            return Result.Failure(ErrorType.BadRequest, "Player name cannot be null or empty!");

        if (Status != GameStatus.WaitingForPlayers)
            return Result.Failure(ErrorType.Conflict, "Game is already full!");

        if (PlayerWhite == null)
            PlayerWhite = playerName;
        else
            PlayerBlack = playerName;

        if (PlayerWhite != null && PlayerBlack != null)
        {
            if (PlayerWhite == PlayerBlack)
                return Result.Failure(ErrorType.BadRequest, $"Player {PlayerWhite} cannot play against themselves!");

            Status = GameStatus.Waiting;
        }

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

    public Result DoMove(string newFen, GameResult? result, string username)
    {
        if (Status != GameStatus.Active)
            return Result.Failure(ErrorType.Conflict, "Game is not active!");

        Players? player = GetPlayer(username);
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

        LastMoveTime = DateTime.UtcNow;
        NextPlayer = (NextPlayer == Players.White) ? Players.Black : Players.White;

        UpdateTime();
        CheckOutOfTime();

        return Result.Success();
    }

    public Result OfferDraw(string username)
    {
        if (Status != GameStatus.Active)
            return Result.Failure(ErrorType.Conflict, "Game is not active!");

        if (GetPlayer(username) == null)
            return new Error(ErrorType.Forbidden, $"Player \"{username}\" is not a participant of this game.");

        if (DrawOffer == null)
            DrawOffer = new DrawOffer(username);
        else
        {
            if (DrawOffer.Player != username)
            {
                MatchResult = GameResult.Draw;
                Status = GameStatus.Done;
            }
            else
                return Result.Failure(ErrorType.Conflict, $"Player \"{username}\" is already offering a draw.");
        }

        return Result.Success();
    }

    public Result Resign(string username)
    {
        if (Status != GameStatus.Active)
            return Result.Failure(ErrorType.Conflict, "Game is not active!");

        Players? player = GetPlayer(username);
        if (player == null)
            return new Error(ErrorType.Forbidden, $"Player \"{username}\" is not a participant of this game.");

        MatchResult = (player == Players.White) ? GameResult.WinBlack : GameResult.WinWhite;
        Status = GameStatus.Done;

        return Result.Success();
    }

    public Result<bool> CheckOutOfTime()
    {
        if (Status != GameStatus.Active)
            return Result<bool>.Failure(ErrorType.Conflict, "Game is not active!");

        Result res = UpdateTime();
        if (res.IsFailure) return Result<bool>.Failure(res.Error);

        if (NextPlayer == Players.White && TimeLeftWhite <= TimeSpan.Zero)
        {
            TimeLeftWhite = TimeSpan.Zero;
            MatchResult = GameResult.WinBlack;
            Status = GameStatus.Done;
            return Result<bool>.Success(true);
        }
        else if (NextPlayer == Players.Black && TimeLeftBlack <= TimeSpan.Zero)
        {
            TimeLeftBlack = TimeSpan.Zero;
            MatchResult = GameResult.WinWhite;
            Status = GameStatus.Done;
            return Result<bool>.Success(true);
        }

        return Result<bool>.Success(false);
    }

    private Result UpdateTime()
    {
        if (Status != GameStatus.Active)
            return Result.Failure(ErrorType.Conflict, "Game is not active!");

        if (NextPlayer == Players.White)
            TimeLeftWhite -= DateTime.UtcNow - lastClockUpdateTime;
        else
            TimeLeftBlack -= DateTime.UtcNow - lastClockUpdateTime;

        lastClockUpdateTime = DateTime.UtcNow;

        return Result.Success();
    }

    private Players? GetPlayer(string username)
    {
        if (username == PlayerWhite)
            return Players.White;
        else if (username == PlayerBlack)
            return Players.Black;
        else
            return null;
    }
}