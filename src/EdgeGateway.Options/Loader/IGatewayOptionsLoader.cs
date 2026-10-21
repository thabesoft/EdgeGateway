using EdgeGateway.Devices;
using EdgeGateway.Options.Serialization;
using EdgeGateway.Ports;
using FluentResults;
using Microsoft.Extensions.Options;

namespace EdgeGateway.Options.Loader;


public interface IGatewayOptionsLoader
{
    ValueTask<Result<GatewayDescriptor>> LoadAsync(CancellationToken cancellationToken = default);
}

internal sealed class GatewayOptionsLoader(
    IOptions<GatewaySourceOptions> options,
    IGatewayOptionsSerializer serializer,
    IEnumerable<IPortOptionsConverter> converters
    ) : IGatewayOptionsLoader
{
    public async ValueTask<Result<GatewayDescriptor>> LoadAsync(CancellationToken cancellationToken = default)
    {
        var optionsFile = options.Value.Path;
        if (!File.Exists(optionsFile)) return Result.Fail($"网关配置文件不存在: [{optionsFile}]");


        using var fs = new FileStream(optionsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var result = await serializer.DeserializeAsync(fs, cancellationToken);
        if (result.IsFailed) return Result.Fail("网关配置文件反序列化失败").WithErrors(result.Errors);


        return Result.Try(() => new GatewayDescriptor()
        {
            Ports = [.. result.Value.Ports.Select(Map)],
            Devices = [.. result.Value.Devices.Select(Map)]
        });
    }

    // 端口 Dto -> Options
    private IPortOptions Map(PortOptionsDto dto)
    {
        var converter = converters.FirstOrDefault(x => x.CanConvert(dto));
        return converter?.Convert(dto) ?? throw new NotSupportedException($"不支持的端口类型: {dto.GetType().Name}");
    }

    // 设备 Dto -> Options
    private static DeviceDescriptor Map(DeviceOptionsDto dto)
    {
        var protocolType = dto.Protocol;
        var portType = dto.PortAddress.Type;
        var portAddress = PortAddress.Create(portType, dto.PortAddress.Location);

        return DeviceDescriptor.Create(protocolType, portAddress);
    }
}
