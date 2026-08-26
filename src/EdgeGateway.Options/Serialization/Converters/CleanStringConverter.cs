using System.Text.Json;
using System.Text.Json.Serialization;

namespace EdgeGateway.Options.Serialization.Converters;

internal sealed class CleanStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return Clean(value);
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(Clean(value));
    }

    private static string Clean(string? value)
    {
        if (value is null) throw new JsonException("值不可为空");

        var trimmed = value.Trim();
        if (string.IsNullOrWhiteSpace(trimmed)) throw new JsonException("值不可为空白");

        return trimmed;
    }
}



