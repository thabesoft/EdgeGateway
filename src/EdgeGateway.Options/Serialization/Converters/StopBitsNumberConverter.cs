using System.IO.Ports;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EdgeGateway.Options.Serialization.Converters;


internal sealed class StopBitsNumberConverter : JsonConverter<StopBits>
{
    public override StopBits Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var number = reader.GetDouble();

        return number switch
        {
            1 => StopBits.One,
            1.5 => StopBits.OnePointFive,
            2 => StopBits.Two,
            _ => throw new JsonException($"无效的停止位: {number}")
        };
    }

    public override void Write(Utf8JsonWriter writer, StopBits value, JsonSerializerOptions options)
    {
        var number = value switch
        {
            StopBits.One => 1,
            StopBits.OnePointFive => 1.5,
            StopBits.Two => 2,
            _ => throw new JsonException($"无效的停止位: {value}，有效值为 1, 1.5, 2")
        };

        writer.WriteStringValue(number.ToString());
    }
}
