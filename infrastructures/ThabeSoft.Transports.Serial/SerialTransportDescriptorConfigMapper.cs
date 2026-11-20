using ThabeSoft.Configuration;
using ThabeSoft.Configuration.Nodes;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Serial;


internal sealed class SerialTransportDescriptorConfigMapper(
        IConfigMapper<RetryDescriptor> retryDescriptorMapper
    ) : IConfigMapper<TransportDescriptor>
{
    public bool CanMap(IConfigNode root)
    {
        if (!Enum.TryParse<TransportType>(root.GetStringByPath("Type"), true, out var enumValue))
            return false;

        return enumValue == TransportType.Serial;
    }

    public TransportDescriptor Map(IConfigNode root)
    {
        var type = root.GetStringByPath("Type").Required("缺少类型")
             .ToEnum<TransportType>();

        if (type != TransportType.Serial)
            throw new InvalidOperationException("不支持");


        var portName = root.GetStringByPath("PortName").Required("缺少串口名称")
                .PortName();

        var baudRate = root.GetStringByPath("BaudRate").Required("缺少波特率")
            .ToInt32()
            .StandardBaudRate();

        var dataBits = root.GetStringByPath("DataBits").Required("缺少数据位")
            .ToInt32()
            .DataBits();

        var parity = root.GetStringByPath("Parity").Required("缺少校验值")
            .ToEnum<SerialParity>();

        var stopBits = root.GetStringByPath("StopBits").Required("缺少停止位")
            .ToEnum<SerialStopBits>();

        var retry = retryDescriptorMapper.Map(root.GetByPath("Retry").Required("缺少重试信息"));

        return new SerialTransportDescriptor()
        {
            PortName = portName,
            BaudRate = baudRate,
            DataBits = dataBits,
            Parity = parity,
            StopBits = stopBits,
            Retry = retry
        };
    }
}