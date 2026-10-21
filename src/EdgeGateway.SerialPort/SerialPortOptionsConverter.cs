using EdgeGateway.Options;
using EdgeGateway.Options.Loader;
using EdgeGateway.Ports;
using EdgeGateway.SerialPort;

namespace EdgeGateway.Ethernet;


internal sealed class SerialPortOptionsConverter : IPortOptionsConverter
{
    public bool CanConvert(PortOptionsDto dto)
    {
        return dto is SerialPortOptionsDto;
    }

    public IPortOptions Convert(PortOptionsDto source)
    {
        if (source is not SerialPortOptionsDto dto)
            throw new NotSupportedException($"不支持的配置类型: {source.GetType().Name}");

        return SerialPortDescriptor.Builder(dto.PortName)
              .SetParity(dto.Parity)
              .SetBaudRate(dto.BaudRate)
              .SetStopBits(dto.StopBits)
              .SetDataBits(dto.DataBits)
              .Build();
    }
}