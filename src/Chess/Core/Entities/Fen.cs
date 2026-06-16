using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Core.Entities;

public static class Fen
{
    public static Result<ChessBoard> FenToBoard(string fen)
    {
        ChessBoard board = new();

        string[] infos = fen.Split(' ');
        if (infos.Length != 6)
            return new Error(ErrorType.BadRequest, $"Expected 6 space-serperated infos in FEN but found {infos.Length}");

        // Parse board
        string[] rows = infos[0].Split('/');
        if (rows.Length != 8)
            return new Error(ErrorType.BadRequest, $"Expected 8 rows for the board in FEN but found {rows.Length}");
        for (int y = 0; y < 8; y++)
            if (InitRow(y, rows[7 - y], board).TryGetError(out var err)) return err; // FEN starts with row 8


        // Parse next player
        if (infos[1] == "w")
            board.NextPlayer = Players.White;
        else if (infos[1] == "b")
            board.NextPlayer = Players.Black;
        else
            return new Error(ErrorType.BadRequest, $"Invalid next player info \"{infos[1]}\"");

        // Parse castling rights
        if (infos[2] != "-")
        {
            if (infos[2].Any(c => c != 'K' && c != 'Q' && c != 'k' && c != 'q'))
                return new Error(ErrorType.BadRequest, $"Invalid castle info \"{infos[2]}\"");

            board.CastleKingWhite = infos[2].Contains('K');
            board.CastleQueenWhite = infos[2].Contains('Q');
            board.CastleKingBlack = infos[2].Contains('k');
            board.CastleQueenBlack = infos[2].Contains('q');
        }

        // Parse en passant target
        string enPassantInfo = infos[3];
        if (enPassantInfo == "-")
            board.EnPassantTarget = null;
        else
        {
            if (FieldDescToPos(enPassantInfo).TryGetValue(out var pos, out var error))
                board.EnPassantTarget = pos;
            else
                return error;
        }

        // Parse half moves since last action
        try { board.HalfMovesSinceAction = int.Parse(infos[4]); }
        catch { return new Error(ErrorType.BadRequest, $"Failed to parse integer \"{infos[4]}\""); }

        // Parse total move count
        try { board.MoveCount = int.Parse(infos[5]); }
        catch { return new Error(ErrorType.BadRequest, $"Failed to parse integer \"{infos[5]}\""); }

        return board;
    }
    private static Result InitRow(int row, string rowFen, ChessBoard board)
    {
        int x = 0;
        foreach (char c in rowFen)
        {
            if (x > 7)
                return new Error(ErrorType.BadRequest, $"Invalid fen for row {row} (\"{rowFen}\"). Overflow.");

            if (!char.IsDigit(c))
            {
                var res = CharToPiece(c).Then(c => board.Fields[x, row] = c);
                if (res.IsFailure) return res.Error;
                x++;
                continue;
            }

            int emptyCount = c - '0';
            for (int i = 0; i < emptyCount; i++)
            {
                if (x > 7)
                    return new Error(ErrorType.BadRequest, $"Invalid fen for row {row} (\"{rowFen}\"). Overflow.");

                board.Fields[x, row] = ChessPiece.Empty;
                x++;

            }
        }

        if (x != 8)
            return new Error(ErrorType.BadRequest, $"Invalid fen for row {row} (\"{rowFen}\"). Not all fields are filled.");

        return Result.Success();
    }
    public static Result<Int2> FieldDescToPos(string str)
    {
        if (str.Length != 2)
            return new Error(ErrorType.BadRequest, $"Field descriptions must have a length of 2");

        int x = char.ToLower(str[0]) - 'a';
        int y = str[1] - '1';
        Int2 pos = new(x, y);

        if (!ChessBoard.InBounds(pos))
            return new Error(ErrorType.BadRequest, $"Invalid field description \"{str}\" (out of bounds)");

        return pos;
    }
    public static Result<ChessPiece> CharToPiece(char c)
    {
        if (c == ' ') return ChessPiece.Empty;

        ChessFigure? figure = c.ToString().ToUpper() switch
        {
            "P" => ChessFigure.Pawn,
            "R" => ChessFigure.Rook,
            "N" => ChessFigure.Knight,
            "B" => ChessFigure.Bishop,
            "Q" => ChessFigure.Queen,
            "K" => ChessFigure.King,
            _ => null
        };
        if (!figure.HasValue)
            return new Error(ErrorType.BadRequest, $"Invalid chess figure '{c}'");

        return new ChessPiece(figure.Value, char.IsUpper(c) ? Players.White : Players.Black);
    }

    public static string BoardToFen(ChessBoard board)
    {
        string fen = "";

        for (int y = 0; y < 8; y++)
            fen += CalcRowFen(7 - y, board) + '/'; // FEN starts with row 8

        fen = fen[..^1]; // Remove trailing '/'

        fen += board.NextPlayer == Players.White ? " w " : " b ";

        string castleInfo = "";
        if (board.CastleKingWhite) castleInfo += 'K';
        if (board.CastleQueenWhite) castleInfo += 'Q';
        if (board.CastleKingBlack) castleInfo += 'k';
        if (board.CastleQueenBlack) castleInfo += 'q';
        fen += (castleInfo == "" ? "-" : castleInfo) + ' ';

        fen += (board.EnPassantTarget == null ? "-" : PosToFieldDesc(board.EnPassantTarget.Value)) + ' ';

        fen += $"{board.HalfMovesSinceAction} {board.MoveCount}";

        return fen;
    }
    private static string CalcRowFen(int row, ChessBoard board)
    {
        string rowFen = "";

        int emptyCount = 0;
        for (int x = 0; x < 8; x++)
        {
            char c = PieceToChar(board.Fields[x, row]);

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
    public static char PieceToChar(ChessPiece piece)
    {
        if (piece == ChessPiece.Empty) return ' ';
        char c = piece.Figure switch
        {
            ChessFigure.Pawn => 'p',
            ChessFigure.Rook => 'r',
            ChessFigure.Knight => 'n',
            ChessFigure.Bishop => 'b',
            ChessFigure.Queen => 'q',
            ChessFigure.King => 'k',
            _ => throw new InvalidOperationException("Invalid chess figure")
        };

        return (piece.Color == Players.White) ? char.ToUpper(c) : char.ToLower(c);
    }
    public static string PosToFieldDesc(Int2 pos)
    {
        char posX = (char)('a' + pos.x);
        char posY = (char)('1' + pos.y);
        return $"{posX}{posY}";
    }
}