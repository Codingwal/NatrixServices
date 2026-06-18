namespace NatrixServices.Chess.Core.Entities;

public readonly record struct EventId
{
    public string Value { get; }
    public EventId(string value)
    {
        if (value.Length != 8)
            throw new ArgumentException("EventId must be 8 characters long.");
        if (value.Any(c => !char.IsLetterOrDigit(c)))
            throw new ArgumentException($"Illegal character in EventId");

        Value = value;
    }

    public static EventId Generate()
    {
        string id = Guid.NewGuid().ToString()[0..8];
        return new EventId(id);
    }

    public static bool TryParse(string? value, out EventId result)
    {
        if (value is null || value.Length != 8)
        {
            result = default;
            return false;
        }

        result = new EventId(value);
        return true;
    }
}