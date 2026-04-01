namespace NatrixServices.Chess;

public partial class ChessEngine
{
    /* Board helper methods */
    private Int2? FindPiece(char piece)
    {
        return FindPiece(x => x == piece);
    }
    private Int2? FindPiece(Func<char, bool> predicate)
    {
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                if (predicate(Game.Fields[x, y]))
                    return new Int2(x, y);
        return null;
    }
    public char GetPiece(Int2 pos)
    {
        return Game.Fields[pos.x, pos.y];
    }
    private bool IsEmpty(Int2 pos)
    {
        return GetPiece(pos) == ' ';
    }
    private void SetField(Int2 pos, char piece)
    {
        Game.Fields[pos.x, pos.y] = piece;
    }

    /* Int2 helper methods */
    private static Int2 GetDirection(Int2 from, Int2 to)
    {
        return Int2.Sign(to - from);
    }
    private static bool IsStraight(Int2 from, Int2 to)
    {
        Int2 diff = to - from;
        return diff.x == 0 || diff.y == 0;
    }
    private static bool IsDiagonal(Int2 from, Int2 to)
    {
        Int2 diff = Int2.Abs(to - from);
        return diff.x == diff.y;
    }


    /* Player helper methods */
    public static Players OtherPlayer(Players player)
    {
        if (player == Players.White)
            return Players.Black;
        else if (player == Players.Black)
            return Players.White;
        else
            throw new Exception($"Invalid player {player}");
    }
    private static Players GetPlayer(char field)
    {
        if (field == ' ')
            return Players.None;

        return char.IsUpper(field) ? Players.White : Players.Black;
    }


    /* Bounds checks */
    private static void CheckBounds(Int2 pos)
    {
        CheckBounds(pos.x);
        CheckBounds(pos.y);
    }
    private static void CheckBounds(int pos)
    {
        if (!InBounds(pos))
            throw new ArgumentException($"Out of bounds field index ({pos} is not between 0 and 7)");
    }
    private static bool InBounds(int pos)
    {
        return 0 <= pos && pos < 8;
    }


    /* To (or from) string */
    public static Int2 FieldDescToPos(string str)
    {
        if (str.Length != 2) throw new ArgumentException($"Field descriptions must have a length of 2");

        int x = char.ToLower(str[0]) - 'a';
        int y = str[1] - '1';
        return new(x, y);
    }
    public static string PosToFieldDesc(Int2 pos)
    {
        char posX = (char)('a' + pos.x);
        char posY = (char)('1' + pos.y);
        return $"{posX}{posY}";
    }
    public void PrintBoard()
    {
        string str = "";
        for (int y = 7; y >= 0; y--)
        {
            str += $"{y + 1}. ";
            for (int x = 0; x < 8; x++)
            {
                str += Game.Fields[x, y] + ' ';
            }
            str += '\n';
        }

        str += "   ";
        for (int x = 0; x < 8; x++)
            str += $"{(char)('a' + x)} ";

        Console.WriteLine(str);
    }
}