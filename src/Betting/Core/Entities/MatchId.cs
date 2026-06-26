using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using NatrixServices.Shared.Core;

namespace NatrixServices.Betting.Core.Entities;

[JsonConverter(typeof(BaseIdJsonConverter<MatchId>))]
public record MatchId : BaseId, IId<MatchId>
{
    private const int length = 8;
    protected override int Length => length;

    public MatchId(string value) : base(value) { }

    public static MatchId Generate() => new MatchId(GenerateId(length));

    public static bool TryParse(string? value, [NotNullWhen(true)] out MatchId? result)
    {
        if (!IsValidId(value, length))
        {
            result = default;
            return false;
        }

        result = new MatchId(value!);
        return true;
    }
}