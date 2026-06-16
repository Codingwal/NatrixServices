using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Core.Entities;

public enum ChessFigure { None, Pawn, Rook, Knight, Bishop, Queen, King }

public record struct ChessPiece
{
    public ChessFigure Figure { get; }
    public Players? Color { get; }

    public static ChessPiece Empty => new(ChessFigure.None, null);

    public ChessPiece(ChessFigure figure, Players? color)
    {
        if (figure == ChessFigure.None != (color == null))
            throw new InvalidOperationException("Color should be null exactly when figure is None");

        Figure = figure;
        Color = color;
    }

    public readonly bool IsColor(Players color)
    {
        if (Figure == ChessFigure.None) return false;
        return Color!.Value == color;
    }
}

public class ChessBoard
{
    public ChessPiece[,] Fields { get; private set; } = new ChessPiece[8, 8];

    public Players NextPlayer { get; set; }

    public bool CastleKingWhite { get; set; }
    public bool CastleQueenWhite { get; set; }
    public bool CastleKingBlack { get; set; }
    public bool CastleQueenBlack { get; set; }

    public Int2? EnPassantTarget { get; set; }
    public int HalfMovesSinceAction { get; set; }  // action: pawn move or capture
    public int MoveCount { get; set; }

    public ChessBoard Copy()
    {
        string fen = Fen.BoardToFen(this);
        var result = Fen.FenToBoard(fen);
        if (result.IsFailure) throw new();
        return result.Value;
    }

    public void DoMove(Move move)
    {
        var piece = GetPiece(move.Origin);
        var capturedPiece = GetPiece(move.Destination);

        if (!piece.Color.HasValue) throw new InvalidOperationException("Can't move empty field");
        Players player = piece.Color.Value;

        SetField(move.Origin, ChessPiece.Empty);
        SetField(move.Destination, piece);

        // Handle en passant capture
        if (piece.Figure == ChessFigure.Pawn && move.Destination == EnPassantTarget)
        {
            Int2 capturedPawnPos = move.Destination + new Int2(0, (player == Players.White) ? -1 : 1);
            capturedPiece = GetPiece(capturedPawnPos);
            SetField(capturedPawnPos, ChessPiece.Empty);
        }

        // Handle promotion
        if (move.Promotion != null)
            SetField(move.Destination, new(move.Promotion.Value, player));

        // Handle castling
        if (piece.Figure == ChessFigure.King && Math.Abs(move.Destination.x - move.Origin.x) == 2)
        {
            int y = (player == Players.White) ? 0 : 7;
            if (move.Destination.x == 6) // Kingside
            {
                // Move the rook
                Fields[5, y] = Fields[7, y];
                Fields[7, y] = ChessPiece.Empty;
            }
            else // Queenside
            {
                // Move the rook
                Fields[3, y] = Fields[0, y];
                Fields[0, y] = ChessPiece.Empty;
            }
        }

        // Switch player
        NextPlayer = (NextPlayer == Players.White) ? Players.Black : Players.White;

        UpdateCastlingRights(move, piece);

        // Update en passant target
        if (piece.Figure == ChessFigure.Pawn && Math.Abs(move.Destination.y - move.Origin.y) == 2)
            EnPassantTarget = move.Origin + new Int2(0, (player == Players.White) ? 1 : -1);
        else
            EnPassantTarget = null;

        // Update the half move clock (reset if a pawn moved or a piece was captured)
        if (capturedPiece != ChessPiece.Empty || piece.Figure == ChessFigure.Pawn)
            HalfMovesSinceAction = 0;
        else
            HalfMovesSinceAction++;

        // MoveCount starts at 1 and is incremented after Black's move
        if (player == Players.Black)
            MoveCount++;
    }

    private void UpdateCastlingRights(Move move, ChessPiece piece)
    {
        if (piece.Color == Players.White && piece.Figure == ChessFigure.King)
        {
            CastleKingWhite = false;
            CastleQueenWhite = false;
        }
        else if (piece.Color == Players.White && piece.Figure == ChessFigure.Rook)
        {
            if (move.Origin == new Int2(0, 0))
                CastleQueenWhite = false;
            else if (move.Origin == new Int2(7, 0))
                CastleKingWhite = false;
        }
        else if (piece.Color == Players.Black && piece.Figure == ChessFigure.King)
        {
            CastleKingBlack = false;
            CastleQueenBlack = false;
        }
        else if (piece.Color == Players.Black && piece.Figure == ChessFigure.Rook)
        {
            if (move.Origin == new Int2(0, 7))
                CastleQueenBlack = false;
            else if (move.Origin == new Int2(7, 7))
                CastleKingBlack = false;
        }
    }

    public ChessPiece GetPiece(Int2 pos)
    {
        if (!InBounds(pos))
            throw new InvalidOperationException($"Requested ChessPiece for out of bounds position ${pos}");

        return Fields[pos.x, pos.y];
    }
    private void SetField(Int2 pos, ChessPiece piece)
    {
        Fields[pos.x, pos.y] = piece;
    }
    public static bool InBounds(Int2 pos)
    {
        return 0 <= pos.x && pos.x < 8
            && 0 <= pos.y && pos.y < 8;
    }
    public Int2? FindPiece(ChessPiece piece)
    {
        return ForEachField()
            .FirstOrDefault(field => field.piece == piece).pos;
    }
    public IEnumerable<(Int2 pos, ChessPiece piece)> ForEachField()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                yield return (new(x, y), Fields[x, y]);
            }
        }
    }
}