using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Serial;


internal sealed class SerialTransportDescriptorConfigMapper(
        IConfigMapper<RetryDescriptor> retryDescriptorMapper
    ) : IConfigMapper<TransportDescriptor>
{
    public bool CanMap(IConfigNode root)
    {
        if (!Enum.TryParse<TransportType>(root.GetValue("Type"), true, out var enumValue))
            return false;

        return enumValue == TransportType.Serial;
    }

    public TransportDescriptor Map(IConfigNode root)
    {
        var type = root.GetValue("Type").Required("缺少类型")
             .ToEnum<TransportType>();

        if (type != TransportType.Serial)
            throw new InvalidOperationException("不支持");


        var portName = root.GetValue("PortName").Required("缺少串口名称")
                .PortName();

        var baudRate = root.GetValue("BaudRate").Required("缺少波特率")
            .ToInt32()
            .StandardBaudRate();

        var dataBits = root.GetValue("DataBits").Required("缺少数据位")
            .ToInt32()
            .DataBits();

        var parity = root.GetValue("Parity").Required("缺少校验值")
            .ToEnum<SerialParity>();

        var stopBits = root.GetValue("StopBits").Required("缺少停止位")
            .ToEnum<SerialStopBits>();

        var retry = retryDescriptorMapper.Map(root.GetChild("Retry").Required(""));

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