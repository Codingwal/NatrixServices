namespace NatrixServices.Shared.Core;

public readonly record struct Int2
{
    public readonly int x;
    public readonly int y;
    public Int2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

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

    public static Int2 Sign(Int2 val)
    {
        return new(Math.Sign(val.x), Math.Sign(val.y));
    }
    public static Int2 Abs(Int2 val)
    {
        return new(Math.Abs(val.x), Math.Abs(val.y));
    }
}