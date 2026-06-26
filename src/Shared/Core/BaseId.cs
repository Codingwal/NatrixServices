using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NatrixServices.Shared.Core;

public interface IId<TId>
{
    public static abstract bool TryParse(string? value, [NotNullWhen(true)] out TId? id);
}

public abstract record BaseId
{
    public string Value { get; }
    protected abstract int Length { get; }
    public BaseId(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!IsValidId(value, Length))
            throw new ArgumentException($"Invalid id \"{value}\".");

        Value = value;
    }

    protected static string GenerateId(int length)
    {
        return Guid.NewGuid().ToString("N")[0..length];
    }

    protected static bool IsValidId(string? value, int expectedLength)
    {
        if (value is null) return false;
        if (value.Length != expectedLength) return false;
        if (value.Any(c => !char.IsLetterOrDigit(c))) return false;
        return true;
    }

    public override string ToString()
    {
        return Value;
    }
}

public class BaseIdJsonConverter<TId> : JsonConverter<TId>
    where TId : BaseId, IId<TId>
{
    public override TId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();

        if (TId.TryParse(value, out TId? id))
            return id;
        else
            throw new JsonException($"Invalid {typeof(TId).Name} \"{value}\"");

    }

    public override void Write(Utf8JsonWriter writer, TId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}