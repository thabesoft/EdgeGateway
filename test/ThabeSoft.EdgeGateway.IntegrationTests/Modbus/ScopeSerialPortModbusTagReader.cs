using EdgeGateway.Serial;
using NModbus;
using ThabeSoft.EdgeGateway.Modbus;

namespace EdgeGateway.Modbus;

public sealed class ScopeSerialPortModbusTagReader : ScopeSerialPort
{
    private readonly IModbusMaster _master;

    internal ModbusTagReader TagReader { get; }


    public ScopeSerialPortModbusTagReader(SerialPortDescriptor descriptor) : base(descriptor)
    {
        _master = ModbusFactory.CreateRtuMaster(new SerialPortStreamResourceWapper(SerialPort));
        TagReader = new(_master);
    }

    public override ValueTask DisposeAsync()
    {
        _master.Dispose();
        return base.DisposeAsync();
    }


    private static ModbusFactory ModbusFactory { get; } = new();
}
