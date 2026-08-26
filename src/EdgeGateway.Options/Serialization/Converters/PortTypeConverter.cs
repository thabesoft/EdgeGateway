using EdgeGateway.Ports;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EdgeGateway.Options.Serialization.Converters;


internal sealed class PortTypeConverter : JsonConverter<PortType>
{
    public override PortType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();

        if (string.IsNullOrWhiteSpace(str))
            throw new JsonException($"无效的 协议 值: 不能为空");

        if (PortType.TryParse(str, System.Globalization.CultureInfo.InvariantCulture, out var result))
            return result;

        throw new JsonException($"无效的 协议 格式: {str}");
    }

    public override void Write(Utf8JsonWriter writer, PortType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}