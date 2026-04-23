namespace NatrixServices.Chess.Core;

public partial class ChessEngine(ChessGame Game)
{
    public char? CheckResult()
    {
        // Handle fifty-move rule
        if (Game.HalfMovesSinceAction >= 100)
            return 'd';

        // Handle checkmate and draw because of no moves
        if (GetAllowedMoves().Count == 0)
        {
            if (InCheck(Game.NextPlayer))
                return (Game.NextPlayer == Players.White) ? 'b' : 'w';
            else
                return 'd';
        }

        return null;
    }

    public string? CheckMove(Move move)
    {
        string? error = CheckMoveNoCheck(move);
        if (error != null) return error;

        char piece = GetPiece(move.Origin);
        char oldDestPiece = GetPiece(move.Destination);

        Game.Fields[move.Origin.x, move.Origin.y] = ' ';
        Game.Fields[move.Destination.x, move.Destination.y] = piece;

        if (InCheck(GetPlayer(piece)))
        {
            // Revert move
            Game.Fields[move.Origin.x, move.Origin.y] = piece;
            Game.Fields[move.Destination.x, move.Destination.y] = oldDestPiece;

            return $"You can't be in check after your move";
        }

        return null;
    }

    public List<Move> GetAllowedMoves()
    {
        List<Move> allowedMoves = [];

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                allowedMoves.AddRange(GetAllowedMoves(new Int2(x, y)));
            }
        }

        return allowedMoves;
    }
    public List<Move> GetAllowedMoves(Int2 pos)
    {
        List<Move> allowedMoves = [];

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Move move = new(pos, new Int2(x, y));
                string? error = CheckMove(move);
                if (error == null)
                    allowedMoves.Add(move);
            }
        }

        return allowedMoves;
    }

    /// <returns>null if the move is valid, otherwise an error message</returns>
    private string? CheckMoveNoCheck(Move move)
    {
        CheckBounds(move.Origin);
        CheckBounds(move.Destination);

        if (IsEmpty(move.Origin))
            return "There is no piece on the starting field";

        char startPiece = GetPiece(move.Origin);
        char destPiece = GetPiece(move.Destination);
        Players player = GetPlayer(startPiece);

        if (player != Game.NextPlayer)
            return $"It's the turn of {Game.NextPlayer}, not yours";

        if (GetPlayer(destPiece) == player)
            return $"Can't capture your own piece";

        string? error = char.ToLower(startPiece) switch
        {
            'p' => CheckPawnMove(move, player),
            'r' or 'b' or 'q' => CheckLineMove(move),
            'n' => CheckKnightMove(move),
            'k' => CheckKingMove(move, player),
            _ => throw new ArgumentException($"Invalid figure '{startPiece}'"),
        };
        if (error != null) return error;

        return CheckPromotion(move, startPiece, player);
    }

    private static string? CheckPromotion(Move move, char piece, Players player)
    {
        int y = (player == Players.White) ? 7 : 0;
        bool lastRow = move.Destination.y == y;
        bool isPawn = char.ToLower(piece) == 'p';

        if (move.Promotion == null)
        {
            if (lastRow && isPawn)
                return "You must promote if a pawn reaches the last row";
            return null;
        }

        if (!isPawn)
            return "Only pawns can be promoted";

        if (!lastRow)
            return "You can only promote if the pawn reached the last row";

        char newPiece = char.ToLower(move.Promotion.Value);
        if (newPiece != 'q' && newPiece != 'r' && newPiece != 'b' && newPiece != 'n')
            return "Invalid promotion piece";

        return null;
    }

    private string? CheckPawnMove(Move move, Players player)
    {
        int moveDir = (player == Players.White) ? 1 : -1;

        // Straight?
        if (move.Destination == move.Origin + new Int2(0, moveDir))
        {
            if (!IsEmpty(move.Destination))
                return "The pawn can not capture straight";
            return null;
        }

        // Double move?
        if (move.Destination == move.Origin + new Int2(0, moveDir * 2))
        {
            if ((player == Players.White && move.Origin.y != 1)
                || (player == Players.Black && move.Origin.y != 6))
            {
                return "Two-step move is only allowed if the pawn is on his starting field";
            }

            if (!IsEmpty(move.Destination))
                return "The pawn can not capture straight";
            return null;
        }

        // Not Diagonal?
        if (move.Destination != move.Origin + new Int2(1, moveDir)
            && move.Destination != move.Origin + new Int2(-1, moveDir))
            return "The pawn can only move straight or diagonal (one field)";

        // Normal capture
        if (!IsEmpty(move.Destination))
            return null;

        if (move.Destination != Game.EnPassantTarget)
            return "The pawn can't move diagonal unless he captures (or en passant)";

        return null;
    }

    private string? CheckLineMove(Move move)
    {
        bool isStraight = IsStraight(move.Origin, move.Destination);
        bool isDiagonal = IsDiagonal(move.Origin, move.Destination);

        switch (char.ToLower(GetPiece(move.Origin)))
        {
            case 'r':
                if (!isStraight) return "The rook can only move straight";
                break;
            case 'b':
                if (!isDiagonal) return "The bishop can only move diagonal";
                break;
            case 'q':
                if (!isStraight && !isDiagonal) return "The queen can only move straight or diagonal";
                break;
            default:
                throw new Exception($"Expected rook, bishop or queen, not '{GetPiece(move.Origin)}'");
        }

        Int2 dir = GetDirection(move.Origin, move.Destination);

        for (Int2 pos = move.Origin + dir; pos != move.Destination; pos += dir)
        {
            if (!IsEmpty(pos))
                return $"Can't jump over pieces (you are trying to jump over {GetPiece(pos)} at {PosToFieldDesc(pos)})";
        }

        return null;
    }

    private static string? CheckKnightMove(Move move)
    {
        Int2 diff = Int2.Abs(move.Destination - move.Origin);

        if (!((diff.x == 1 && diff.y == 2) || (diff.x == 2 && diff.y == 1)))
            return $"The knight must move in an L shape";
        return null;
    }

    private string? CheckKingMove(Move move, Players player)
    {
        Int2 diff = move.Destination - move.Origin;

        string? castleError = CheckCastling(move, player, out bool castle);
        if (castle) return castleError;

        if (Math.Abs(diff.x) > 1 || Math.Abs(diff.y) > 1)
            return $"The king can only move one field";

        return null;
    }
    private string? CheckCastling(Move move, Players player, out bool castle)
    {
        Int2 diff = move.Destination - move.Origin;
        int y = (player == Players.White) ? 0 : 7;

        if (diff == new Int2(-2, 0))
        {
            castle = true;

            if ((player == Players.White && !Game.CastleQueenWhite) || (player == Players.Black && !Game.CastleQueenBlack))
                return "You can't castle queenside anymore";

            if (!IsEmpty(new Int2(1, y)) || !IsEmpty(new Int2(2, y)) || !IsEmpty(new Int2(3, y)))
                return "You can't castle through or into other pieces";

            if (InCheck(new Int2(4, y), player) || InCheck(new Int2(3, y), player))
                return "You can't castle through check";
        }
        else if (diff == new Int2(2, 0))
        {
            castle = true;

            if ((player == Players.White && !Game.CastleKingWhite) || (player == Players.Black && !Game.CastleKingBlack))
                return "You can't castle kingside anymore";

            if (!IsEmpty(new Int2(5, y)) || !IsEmpty(new Int2(6, y)))
                return "You can't castle through or into other pieces";

            if (InCheck(new Int2(4, y), player) || InCheck(new Int2(5, y), player))
                return "You can't castle through check";
        }
        else
            castle = false;

        return null;
    }

    private bool InCheck(Players player)
    {
        Int2 kingPos = FindPiece(player == Players.White ? 'K' : 'k') ?? throw new Exception($"Could not find king of player {player}");

        return InCheck(kingPos, player);
    }

    private bool InCheck(Int2 pos, Players player)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                char piece = Game.Fields[x, y];
                if (GetPlayer(piece) != OtherPlayer(player))
                    continue;

                if (CheckMove(new Move(new Int2(x, y), pos)) == null)
                    return true;
            }
        }
        return false;
    }
}