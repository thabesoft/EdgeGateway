using EdgeGateway.Modbus.Device;
using EdgeGateway.Serial;
using NModbus;

namespace EdgeGateway.Modbus;

public sealed class ScopeSerialPortModbusTagProtocol : ScopeSerialPort
{
    private readonly IModbusMaster _master;
    internal ModbusTagProtocol TagProtocol { get; }


    public ScopeSerialPortModbusTagProtocol(SerialPortDescriptor descriptor) : base(descriptor)
    {
        _master = ModbusFactory.CreateRtuMaster(new SerialPortStreamResourceWapper(SerialPort));
        TagProtocol = new(_master);
    }

    public override ValueTask DisposeAsync()
    {
        _master.Dispose();
        return base.DisposeAsync();
    }


    private static ModbusFactory ModbusFactory { get; } = new();
}
