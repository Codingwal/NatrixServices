namespace NatrixServices.Chess;

public partial class ChessGame
{
    public ChessPiece[,] Fields { get; private set; } = new ChessPiece[8, 8];
    public Players NextPlayer { get; private set; } = Players.White;
    public ChessGame()
    {
        CreateEdgeRow(0, Players.White);
        CreateRow(1, Figures.Pawn, Players.White);
        CreateRow(2, Figures.None, Players.None);
        CreateRow(3, Figures.None, Players.None);
        CreateRow(4, Figures.None, Players.None);
        CreateRow(5, Figures.None, Players.None);
        CreateRow(6, Figures.Pawn, Players.Black);
        CreateEdgeRow(7, Players.Black);
    }
    private void CreateEdgeRow(int row, Players player)
    {
        Fields[0, row] = new(new(0, row), Figures.Rook, player);
        Fields[1, row] = new(new(1, row), Figures.Knight, player);
        Fields[2, row] = new(new(2, row), Figures.Bishop, player);
        Fields[3, row] = new(new(3, row), Figures.Queen, player);
        Fields[4, row] = new(new(4, row), Figures.King, player);
        Fields[5, row] = new(new(5, row), Figures.Bishop, player);
        Fields[6, row] = new(new(6, row), Figures.Knight, player);
        Fields[7, row] = new(new(7, row), Figures.Rook, player);
    }
    private void CreateRow(int row, Figures figure, Players player)
    {
        for (int i = 0; i < 8; i++)
            Fields[i, row] = new ChessPiece(new(i, row), figure, player);
    }

    public void DoMove(string start, string dest)
    {
        DoMove(FieldDescToPos(start), FieldDescToPos(dest));
    }
    public void DoMove(Int2 start, Int2 dest)
    {
        ChessPiece piece = GetPiece(start) with { };

        DoMove(new Move(piece, start, dest));
    }
    public void DoMove(Move move)
    {
        CheckMove(move);

        ChessPiece old = GetPiece(move.Destination);

        Fields[move.Origin.x, move.Origin.y] = new ChessPiece(move.Origin);
        Fields[move.Destination.x, move.Destination.y] = new ChessPiece(move.Destination, move.Piece.Figure, move.Piece.Player);

        if (InCheck(move.Piece.Player))
        {
            // Revert move
            Fields[move.Origin.x, move.Origin.y] = new ChessPiece(move.Origin, move.Piece.Figure, move.Piece.Player);
            Fields[move.Destination.x, move.Destination.y] = old;

            throw new IllegalMoveException($"You can't be in check after your move");
        }

        NextPlayer = OtherPlayer(NextPlayer);
    }

    public void CheckMove(Move move)
    {
        CheckBounds(move.Origin);
        CheckBounds(move.Destination);

        if (move.Piece.Player == Players.None)
            throw new ArgumentException($"There is no piece on the starting field");

        if (move.Piece.Player != NextPlayer)
            throw new ArgumentException($"It's the turn of {NextPlayer}, not yours");

        ChessPiece piece = GetPiece(move.Origin);

        if (piece != move.Piece)
            throw new ArgumentException($"The specified piece is not the piece on the origin field ({move.Piece} != {piece})");

        if (GetPiece(move.Destination).Player == move.Piece.Player)
            throw new IllegalMoveException($"Can't capture your own piece");

        switch (piece.Figure)
        {
            case Figures.Pawn:
                CheckPawnMove(move);
                break;
            case Figures.Rook:
            case Figures.Bishop:
            case Figures.Queen:
                CheckLineMove(move);
                break;
            case Figures.Knight:
                CheckKnightMove(move);
                break;
            case Figures.King:
                CheckKingMove(move);
                break;
            default:
                throw new ArgumentException($"Invalid figure {piece.Figure}");
        }
    }

    private void CheckPawnMove(Move move)
    {
        int moveDir = move.Piece.Player == Players.White ? 1 : -1;

        // Straight?
        if (move.Destination == move.Origin + new Int2(0, moveDir))
        {
            if (!FieldEmpty(move.Destination))
                throw new IllegalMoveException("The pawn can't capture straight");
            return;
        }

        if (move.Destination == move.Origin + new Int2(0, moveDir * 2))
        {
            if ((move.Piece.Player == Players.White && move.Origin.y != 1)
                || (move.Piece.Player == Players.Black && move.Origin.y != 6))
            {
                throw new IllegalMoveException("Two-step move is only allowed if the pawn is on his starting field");
            }

            if (!FieldEmpty(move.Destination))
                throw new IllegalMoveException("The pawn can't capture straight");
            return;
        }

        // Diagonal?
        if (move.Destination != move.Origin + new Int2(1, moveDir)
            && move.Destination != move.Origin + new Int2(-1, moveDir))
            throw new IllegalMoveException("The pawn can only move straight or diagonal (one field)");

        ChessPiece destPiece = GetPiece(move.Destination);

        // Must belong to the opposition
        if (destPiece.Player != OtherPlayer(move.Piece.Player))
            throw new IllegalMoveException("The pawn can't move diagonal unless he captures");
    }

    private void CheckLineMove(Move move)
    {
        bool isStraight = IsStraight(move.Origin, move.Destination);
        bool isDiagonal = IsDiagonal(move.Origin, move.Destination);

        switch (move.Piece.Figure)
        {
            case Figures.Rook:
                if (!isStraight) throw new IllegalMoveException("The rook can only move straight");
                break;
            case Figures.Bishop:
                if (!isDiagonal) throw new IllegalMoveException("The bishop can only move diagonal");
                break;
            case Figures.Queen:
                if (!isStraight && !isDiagonal) throw new IllegalMoveException("The queen can only move straight or diagonal");
                break;
            default:
                throw new Exception($"Expected rook, bishop or queen, not {move.Piece.Figure}");
        }

        Int2 dir = GetDirection(move.Origin, move.Destination);

        for (Int2 pos = move.Origin + dir; pos != move.Destination; pos += dir)
        {
            ChessPiece piece = GetPiece(pos);

            if (pos == move.Destination)
                return;

            if (piece.Figure != Figures.None)
                throw new IllegalMoveException($"Can't jump over pieces (you are trying to jump over {PieceDescription(piece)})");
        }
    }

    private static void CheckKnightMove(Move move)
    {
        Int2 diff = Int2.Abs(move.Destination - move.Origin);

        if (!((diff.x == 1 && diff.y == 2) || (diff.x == 2 && diff.y == 1)))
            throw new IllegalMoveException($"The knight must move in an L shape");
    }

    private static void CheckKingMove(Move move)
    {
        Int2 diff = move.Destination - move.Origin;

        if (Math.Abs(diff.x) > 1 || Math.Abs(diff.y) > 1)
            throw new IllegalMoveException($"The king can only move one field");
    }

    private bool InCheck(Players player)
    {
        ChessPiece king = FindPiece(Figures.King, player) ?? throw new Exception($"Could not find king of player {player}");

        foreach (ChessPiece piece in Fields)
        {
            if (piece.Player != OtherPlayer(player))
                continue;

            if (CanMakeMove(new Move(piece, piece.Pos, king.Pos)))
                return true;
        }
        return false;
    }

    private bool CanMakeMove(Move move)
    {
        try
        {
            CheckMove(move);
            return true;
        }
        catch
        {
            return false;
        }
    }
}