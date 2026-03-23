using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class Extensions
{
    public static void SaveAsJson<T>(this PropertyBuilder<T> propertyBuilder)
        where T : class, new()
    {
        propertyBuilder.HasConversion(
            obj => JsonSerializer.Serialize(obj),
            str => JsonSerializer.Deserialize<T>(str) ?? new T()
        );

        var comparer = new ValueComparer<T>(
            (lhs, rhs) => JsonSerializer.Serialize(lhs) == JsonSerializer.Serialize(rhs), // Compare
            obj => obj == null ? 0 : JsonSerializer.Serialize(obj).GetHashCode()          // HashCode
        );

        propertyBuilder.Metadata.SetValueComparer(comparer);
    }
}