using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EdgeGateway.Options.Serialization.Converters;


internal sealed class IPEndPointConverter : JsonConverter<IPEndPoint>
{
    public override IPEndPoint? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (string.IsNullOrWhiteSpace(str))
            throw new JsonException("IPEndPoint 值不能为空");

        if (!IPEndPoint.TryParse(str, out var endpoint))
            throw new JsonException($"无效的 IPEndPoint 格式: {str}");

        return endpoint;
    }

    public override void Write(Utf8JsonWriter writer, IPEndPoint value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}