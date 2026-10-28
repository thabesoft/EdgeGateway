using EdgeGateway.Serial;
using NModbus;

namespace EdgeGateway.Modbus;

public sealed class ScopeSerialPortModbusSlaveNetwork : ScopeSerialPort
{
    private readonly IModbusSlaveNetwork _modbusSlaveNetwork;
    private Task? _listenTask;
    private CancellationTokenSource _listenTaskCts = new();

    public bool Listening { get; private set; }


    public ScopeSerialPortModbusSlaveNetwork(SerialPortDescriptor descriptor) : base(descriptor)
    {
        _modbusSlaveNetwork = ModbusFactory.CreateRtuSlaveNetwork(new SerialPortStreamResourceWapper(SerialPort));
    }


    public IModbusSlave AddSlave(byte slaveId)
    {
        var slave = ModbusFactory.CreateSlave(slaveId);
        _modbusSlaveNetwork.AddSlave(slave);

        return slave;
    }

    public override async ValueTask OpenAsync()
    {
        await base.OpenAsync();
        _listenTask = Task.Run(async () => await _modbusSlaveNetwork.ListenAsync(_listenTaskCts.Token));
    }
    public override ValueTask CloseAsync()
    {
        //_modbusSlaveNetwork.Dispose();
        return base.CloseAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();

        _listenTaskCts.Cancel();
        _modbusSlaveNetwork.Dispose();

        try { if (_listenTask is not null) await _listenTask; }
        catch { }

        await Task.Delay(2000);
    }


    private static ModbusFactory ModbusFactory { get; } = new();
}
