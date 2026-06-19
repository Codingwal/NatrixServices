namespace NatrixServices.Chess.Core.Entities;

public readonly record struct GameId
{
    public string Value { get; }
    public GameId(string value)
    {
        if (value.Length != 8)
            throw new ArgumentException("GameId must be 8 characters long.");
        if (value.Any(c => !char.IsLetterOrDigit(c)))
            throw new ArgumentException($"Illegal character in EventId");

        Value = value;
    }

    public static GameId Generate()
    {
        string id = Guid.NewGuid().ToString()[0..8];
        return new GameId(id);
    }

    public static bool TryParse(string? value, out GameId result)
    {
        if (value is null || value.Length != 8 || value.Any(c => !char.IsLetterOrDigit(c)))
        {
            result = default;
            return false;
        }

        result = new GameId(value);
        return true;
    }
}