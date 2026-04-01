namespace NatrixServices.Chess;

public struct Int2(int _x, int _y)
{
    public int x = _x;
    public int y = _y;

    public static Int2 operator +(Int2 a, Int2 b)
    {
        return new(a.x + b.x, a.y + b.y);
    }
    public static Int2 operator -(Int2 a)
    {
        return new(-a.x, -a.y);
    }
    public static Int2 operator -(Int2 a, Int2 b)
    {
        return a + (-b);
    }
    public static bool operator ==(Int2 a, Int2 b)
    {
        return a.x == b.x && a.y == b.y;
    }
    public static bool operator !=(Int2 a, Int2 b)
    {
        return !(a == b);
    }

    public static Int2 Sign(Int2 val)
    {
        return new(Math.Sign(val.x), Math.Sign(val.y));
    }
    public static Int2 Abs(Int2 val)
    {
        return new(Math.Abs(val.x), Math.Abs(val.y));
    }

    public override readonly bool Equals(object? obj)
    {
        if (obj == null)
            return false;
        if (obj is not Int2 other)
            return false;

        return this == other;
    }

    public override readonly int GetHashCode()
    {
        return x + 17 * y;
    }
}