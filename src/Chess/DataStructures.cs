namespace NatrixServices.Chess;

public class IllegalMoveException(string msg) : Exception(msg)
{
}

public enum Figures
{
    None,
    Pawn,
    Rook,
    Knight,
    Bishop,
    King,
    Queen
}
public enum Players
{
    None,
    White,
    Black
}
public record ChessPiece(Int2 Pos, Figures Figure = Figures.None, Players Player = Players.None)
{
    public char FigureSymbol()
    {
        return Figure switch
        {
            Figures.Pawn => 'P',
            Figures.Rook => 'R',
            Figures.Knight => 'N',
            Figures.Bishop => 'B',
            Figures.Queen => 'Q',
            Figures.King => 'K',
            _ => '-',
        };
    }
    public char PlayerSymbol()
    {
        return Player switch
        {
            Players.White => 'W',
            Players.Black => 'B',
            _ => throw new Exception($"Invalid player {Player}")
        };
    }
    public override string ToString()
    {
        if (Figure == Figures.None)
            return " -  ";

        return $"{FigureSymbol()}({PlayerSymbol()})";
    }
}
public record Move(ChessPiece Piece, Int2 Origin, Int2 Destination);
