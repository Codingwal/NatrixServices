using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NatrixServices.Shared.Infrastructure.Database;

public static class SaveAsJsonExtension
{
    public static void SaveAsJson<T>(this PropertyBuilder<T> propertyBuilder)
    {
        propertyBuilder.HasConversion(
            obj => JsonSerializer.Serialize(obj),
            str => JsonSerializer.Deserialize<T>(str) ?? default!
        );

        var comparer = new ValueComparer<T>(
            (lhs, rhs) => JsonSerializer.Serialize(lhs) == JsonSerializer.Serialize(rhs), // Compare
            obj => obj == null ? 0 : JsonSerializer.Serialize(obj).GetHashCode()          // HashCode
        );

        propertyBuilder.Metadata.SetValueComparer(comparer);
    }
}