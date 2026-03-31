namespace NatrixServices.Chess;

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

        fen = fen[..^1]; // Remove trailing '/

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
}