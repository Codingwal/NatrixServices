using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NatrixServices.Shared.Infrastructure.Database;

public static class SaveAsJsonExtension
{
    private static readonly JsonSerializerOptions options = new()
    {
        IgnoreReadOnlyProperties = true
    };
    public static void SaveAsJson<T>(this PropertyBuilder<T> propertyBuilder)
    {
        propertyBuilder.HasConversion(
            obj => JsonSerializer.Serialize(obj, options),
            str => JsonSerializer.Deserialize<T>(str) ?? default!
        );

        var comparer = new ValueComparer<T>(
            (lhs, rhs) => JsonSerializer.Serialize(lhs, options) == JsonSerializer.Serialize(rhs, options), // Compare
            obj => obj == null ? 0 : JsonSerializer.Serialize(obj, options).GetHashCode()          // HashCode
        );

        propertyBuilder.Metadata.SetValueComparer(comparer);
    }
}