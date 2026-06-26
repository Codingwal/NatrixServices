using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Core.Entities;

[JsonConverter(typeof(BaseIdJsonConverter<EventId>))]
public record EventId : BaseId, IId<EventId>
{
    private const int length = 8;
    protected override int Length => length;

    public EventId(string value) : base(value) { }

    public static EventId Generate() => new EventId(GenerateId(length));

    public static bool TryParse(string? value, [NotNullWhen(true)] out EventId? result)
    {
        if (!IsValidId(value, length))
        {
            result = default;
            return false;
        }

        result = new EventId(value!);
        return true;
    }
}