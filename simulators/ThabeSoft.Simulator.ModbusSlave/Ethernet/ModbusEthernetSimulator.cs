using Microsoft.Extensions.Hosting;
using NModbus;
using System.Net;
using System.Net.Sockets;

namespace ThabeSoft.Simulator.Ethernet;


internal sealed class ModbusEthernetSimulator : IModbusSimulator, IHostedService
{
    private readonly TcpListener _tcpListener;
    private readonly IModbusTcpSlaveNetwork _slaveNetWorks;

    public ModbusEthernetSimulator(ModbusEthernetSimulatorDescriptor descriptor, IModbusFactory modbusFactory)
    {
        UnitId = descriptor.UnitId;


        _tcpListener = new(descriptor.Ip, descriptor.Port);
        _slaveNetWorks = modbusFactory.CreateSlaveNetwork(_tcpListener);
    }

    public byte UnitId { get; }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _tcpListener.Start();
        await _slaveNetWorks.ListenAsync(cancellationToken);
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _slaveNetWorks.Dispose();
        _tcpListener.Stop();
        return Task.CompletedTask;
    }
    public ValueTask DisposeAsync()
    {
        _slaveNetWorks.Dispose();
        _tcpListener?.Dispose();
        return ValueTask.CompletedTask;
    }


    public void SetCoils(ushort address, params bool[] values)
    {
        modbusSlave.DataStore.CoilInputs.WritePoints(address, value);
    }

    public void SetHoldingRegisters(ushort address, params ushort[] values)
    {
        throw new NotImplementedException();
    }
}

public sealed class TcpListenerManager
{
    public void Register(IPAddress ip, int port = 0)
    {
        return new TcpListener(ip, port);
    }
}