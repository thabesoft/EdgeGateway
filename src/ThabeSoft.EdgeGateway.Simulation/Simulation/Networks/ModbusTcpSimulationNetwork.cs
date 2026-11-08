using NModbus;
using System.Net.Sockets;
using ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Simulation.Networks;


internal sealed class ModbusTcpSimulationNetwork : ModbusSimulationNetwork
{
    private readonly TcpListener _tcpListener;
    private Task? _listenTask;
    private readonly IModbusTcpSlaveNetwork _slaveNetWorks;


    public ModbusTcpSimulationNetwork(
            IpTransportDescriptor ipTransportDescriptor,
            IModbusFactory modbusFactory,
            ModbusSimulatedSlaveFactory slaveSimulatorFactory
        ) : base(slaveSimulatorFactory)
    {
        _tcpListener = new(ipTransportDescriptor.IPEndPoint);
        _slaveNetWorks = modbusFactory.CreateSlaveNetwork(_tcpListener);
    }

    public byte UnitId { get; }



    protected override void OnAdded(ModbusSimulatedSlave simulated)
    {
        _slaveNetWorks.AddSlave(simulated.Slave);
    }
    protected override void OnRemoved(ModbusSimulatedSlave slave)
    {
        _slaveNetWorks.RemoveSlave(slave.UnitId);
    }


    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        _tcpListener.Start();
        _listenTask = Task.Run(() => _slaveNetWorks.ListenAsync(cancellationToken), cancellationToken);

        await base.OnStartAsync(cancellationToken);
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        await base.OnStopAsync(cancellationToken);
        _tcpListener.Stop();
    }
    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();

        _slaveNetWorks.Dispose();

#if NETSTANDARD2_0
        _tcpListener?.Stop();
#else

        _tcpListener?.Dispose();
#endif
    }
}