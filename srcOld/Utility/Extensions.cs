using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class Extensions
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

    public static void SetUsername(this HttpContext context, string username)
    {
        context.Items["username"] = username;
    }
    public static string GetUsername(this HttpContext context)
    {
        return context.Items["username"] as string ?? throw new Exception("Username not found in HttpContext");
    }
}