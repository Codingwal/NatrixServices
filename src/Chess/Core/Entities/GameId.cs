using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using NatrixServices.Shared.Core;

namespace NatrixServices.Chess.Core.Entities;

[JsonConverter(typeof(BaseIdJsonConverter<GameId>))]
public record GameId : BaseId, IId<GameId>
{
    private const int length = 8;
    protected override int Length => length;

    public GameId(string value) : base(value) { }

    public static GameId Generate() => new GameId(GenerateId(length));

    public static bool TryParse(string? value, [NotNullWhen(true)] out GameId? result)
    {
        if (!IsValidId(value, length))
        {
            result = default;
            return false;
        }

        result = new GameId(value!);
        return true;
    }
}