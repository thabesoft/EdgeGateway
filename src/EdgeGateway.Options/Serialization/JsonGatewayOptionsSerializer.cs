using EdgeGateway.Options.Serialization.Converters;
using FluentResults;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EdgeGateway.Options.Serialization;


internal sealed class JsonGatewayOptionsSerializer : IGatewayOptionsSerializer
{
    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="NotSupportedException"/>
    /// <exception cref="OperationCanceledException"/>
    public async ValueTask<Result<GatewayOptionsDto>> DeserializeAsync(Stream source, CancellationToken cancellationToken = default)
    {
        if (source is null) return Result.Fail("配置反序列化失败: Json 来源为空");

        try
        {
            var dto = await JsonSerializer.DeserializeAsync(source, GatewayJsonContext.Default.GatewayOptionsDto, cancellationToken);
            return dto is null ? Result.Fail("配置反序列化失败: 结果为空") : Result.Ok(dto);
        }
        catch (JsonException ex)
        {
            return Result.Fail<GatewayOptionsDto>("配置反序列化失败: JSON 格式错误")
                .WithError(new ExceptionalError(ex));
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="NotSupportedException"/>
    /// <exception cref="OperationCanceledException"/>
    public async ValueTask<Result> SerializeAsync(Stream destination, GatewayOptionsDto options, CancellationToken cancellationToken = default)
    {
        if (destination is null) return Result.Fail("配置序列化失败: 写入目标为空");

        await JsonSerializer.SerializeAsync(destination, options, GatewayJsonContext.Default.GatewayOptionsDto, cancellationToken);
        return Result.Ok();
    }
}


[JsonSourceGenerationOptions(
        // 忽略大小写，允许驼峰和帕斯卡命名
        PropertyNameCaseInsensitive = true,
        // 格式化输出
        WriteIndented = true
    )]
[JsonSerializable(typeof(GatewayOptionsDto))]
[JsonSerializable(typeof(PortOptionsDto))]
[JsonSerializable(typeof(DeviceOptionsDto))]
[JsonSerializable(typeof(PortAddressDto))]
[JsonSerializable(typeof(TagOptionsDto))]
[JsonSerializable(typeof(DataAddressDto))]
internal partial class GatewayJsonContext : JsonSerializerContext;