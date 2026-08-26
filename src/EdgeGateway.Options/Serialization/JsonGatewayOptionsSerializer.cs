using EdgeGateway.Devices;
using EdgeGateway.Dto;
using FluentResults;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EdgeGateway.Serialization;


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
            return Result.Fail<GatewayOptionsDto>($"配置反序列化失败: JSON 格式错误")
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


[JsonSerializable(typeof(GatewayOptionsDto))]
[JsonSerializable(typeof(DeviceOptionsDto))]
[JsonSerializable(typeof(PortOptionsDto))]
internal partial class GatewayJsonContext : JsonSerializerContext;