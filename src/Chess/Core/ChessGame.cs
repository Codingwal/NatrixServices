namespace NatrixServices.Chess.Core;

public enum Players
{
    None,
    White,
    Black
}
public record Move(Int2 Origin, Int2 Destination, char? Promotion = null);

public partial class ChessGame
{
    public char[,] Fields { get; private set; }

    public Players NextPlayer { get; set; }

    public bool CastleKingWhite { get; set; }
    public bool CastleQueenWhite { get; set; }
    public bool CastleKingBlack { get; set; }
    public bool CastleQueenBlack { get; set; }

    public Int2? EnPassantTarget { get; set; }
    public int HalfMovesSinceAction { get; set; }  // action: pawn move or capture
    public int MoveCount { get; set; }

    public static string DefaultFen => "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    public ChessGame(string fen)
    {
        string[] infos = fen.Split(' ');

        string[] rows = infos[0].Split('/');
        Fields = new char[8, 8];
        for (int y = 0; y < 8; y++)
            InitRow(y, rows[y]);

        NextPlayer = infos[1] == "w" ? Players.White : Players.Black;

        string castleInfo = infos[2];
        CastleKingWhite = castleInfo.Contains('K');
        CastleQueenWhite = castleInfo.Contains('Q');
        CastleKingBlack = castleInfo.Contains('k');
        CastleQueenBlack = castleInfo.Contains('q');

        string enPassantInfo = infos[3];
        EnPassantTarget = enPassantInfo == "-" ? null : ChessEngine.FieldDescToPos(enPassantInfo);

        HalfMovesSinceAction = int.Parse(infos[4]);

        MoveCount = int.Parse(infos[5]);
    }
    private void InitRow(int row, string rowFen)
    {
        int x = 0;
        foreach (char c in rowFen)
        {
            if (!char.IsDigit(c))
            {
                Fields[x, row] = c;
                x++;
                continue;
            }

            int emptyCount = c - '0';
            for (int i = 0; i < emptyCount; i++)
            {
                Fields[x, row] = ' ';
                x++;
            }
        }
    }

    public string ToFen()
    {
        string fen = "";
        for (int y = 0; y < 8; y++)
            fen += CalcRowFen(y) + '/';

        fen = fen[..^1]; // Remove trailing '/'

        fen += NextPlayer == Players.White ? " w " : " b ";

        string castleInfo = "";
        if (CastleKingWhite) castleInfo += 'K';
        if (CastleQueenWhite) castleInfo += 'Q';
        if (CastleKingBlack) castleInfo += 'k';
        if (CastleQueenBlack) castleInfo += 'q';
        fen += (castleInfo == "" ? "-" : castleInfo) + ' ';

        fen += (EnPassantTarget == null ? "-" : ChessEngine.PosToFieldDesc(EnPassantTarget.Value)) + ' ';

        fen += $"{HalfMovesSinceAction} {MoveCount}";

        return fen;
    }
    private string CalcRowFen(int row)
    {
        string rowFen = "";

        int emptyCount = 0;
        for (int x = 0; x < 8; x++)
        {
            char c = Fields[x, row];

            if (c == ' ')
            {
                emptyCount++;
                continue;
            }

            if (emptyCount > 0)
            {
                rowFen += emptyCount;
                emptyCount = 0;
            }

            rowFen += c;
        }

        if (emptyCount > 0)
            rowFen += emptyCount;

        return rowFen;
    }

    public void ForEachPiece(Action<char, Int2> action)
    {
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; x++)
                action(Fields[x, y], new Int2(x, y));
    }
    public int CountPieces(char piece, Players player = Players.None)
    {
        if (player == Players.White)
            piece = char.ToUpper(piece);
        else
            piece = char.ToLower(piece);

        int count = 0;
        ForEachPiece((c, _) =>
        {
            if (c == piece)
                count++;
        });
        return count;
    }

    public void DoMove(Move move)
    {
        // Get move info before executing it
        char piece = GetPiece(move.Origin);
        Players player = GetPlayer(piece);
        char capturedPiece = GetPiece(move.Destination);

        // Update the board
        SetField(move.Destination, piece);
        SetField(move.Origin, ' ');

        // Handle en passant capture
        if (char.ToLower(piece) == 'p' && move.Destination == EnPassantTarget)
        {
            Int2 capturedPawnPos = move.Destination + new Int2(0, (player == Players.White) ? -1 : 1);
            capturedPiece = GetPiece(capturedPawnPos);
            SetField(capturedPawnPos, ' ');
        }

        // Handle promotion
        if (move.Promotion != null)
        {
            if (player == Players.White)
                SetField(move.Destination, char.ToUpper(move.Promotion.Value));
            else
                SetField(move.Destination, char.ToLower(move.Promotion.Value));
        }

        // Handle castling
        if (char.ToLower(piece) == 'k' && Math.Abs(move.Destination.x - move.Origin.x) == 2)
        {
            int y = (player == Players.White) ? 0 : 7;
            if (move.Destination.x == 6) // Kingside
            {
                SetField(new Int2(5, y), GetPiece(new Int2(7, y)));
                SetField(new Int2(7, y), ' ');
            }
            else // Queenside
            {
                SetField(new Int2(3, y), GetPiece(new Int2(0, y)));
                SetField(new Int2(0, y), ' ');
            }
        }

        // Switch player
        NextPlayer = NextPlayer == Players.White ? Players.Black : Players.White;

        UpdateCastlingRights(move, piece);

        // Update en passant target
        if (char.ToLower(piece) == 'p' && Math.Abs(move.Destination.y - move.Origin.y) == 2)
            EnPassantTarget = move.Origin + new Int2(0, (player == Players.White) ? 1 : -1);
        else
            EnPassantTarget = null;

        // Update the half move clock (reset if a pawn moved or a piece was captured)
        if (capturedPiece != ' ' || char.ToLower(piece) == 'p')
            HalfMovesSinceAction = 0;
        else
            HalfMovesSinceAction++;

        // MoveCount starts at 1 and is incremented after Black's move
        if (player == Players.Black)
            MoveCount++;
    }

    private void UpdateCastlingRights(Move move, char piece)
    {
        if (piece == 'K')
        {
            CastleKingWhite = false;
            CastleQueenWhite = false;
        }
        else if (piece == 'R')
        {
            if (move.Origin == new Int2(0, 0))
                CastleQueenWhite = false;
            else if (move.Origin == new Int2(7, 0))
                CastleKingWhite = false;
        }
        else if (piece == 'k')
        {
            CastleKingBlack = false;
            CastleQueenBlack = false;
        }
        else if (piece == 'r')
        {
            if (move.Origin == new Int2(0, 7))
                CastleQueenBlack = false;
            else if (move.Origin == new Int2(7, 7))
                CastleKingBlack = false;
        }
    }

    private char GetPiece(Int2 pos)
    {
        return Fields[pos.x, pos.y];
    }
    private bool IsEmpty(Int2 pos)
    {
        return GetPiece(pos) == ' ';
    }
    private void SetField(Int2 pos, char piece)
    {
        Fields[pos.x, pos.y] = piece;
    }
    private static Players GetPlayer(char field)
    {
        if (field == ' ')
            return Players.None;

        return char.IsUpper(field) ? Players.White : Players.Black;
    }
}