using EdgeGateway.Modbus.Tag;
using EdgeGateway.Serial;
using NModbus;

namespace EdgeGateway.Modbus;

public sealed class ScopeSerialPortModbusTagWriter : ScopeSerialPort
{
    private readonly IModbusMaster _master;

    internal ModbusTagWriter TagWriter { get; }


    public ScopeSerialPortModbusTagWriter(SerialPortDescriptor descriptor) : base(descriptor)
    {
        _master = ModbusFactory.CreateRtuMaster(new SerialPortStreamResourceWapper(SerialPort));
        TagWriter = new(_master);
    }

    public override ValueTask DisposeAsync()
    {
        _master.Dispose();
        return base.DisposeAsync();
    }


    private static ModbusFactory ModbusFactory { get; } = new();
}
