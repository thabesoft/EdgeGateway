using Microsoft.Extensions.Logging;
using NModbus;
using System.Net.Sockets;
using ThabeSoft.EdgeGateway.Ethernet;
using ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;

namespace ThabeSoft.EdgeGateway.Simulation.Networks;


internal sealed class ModbusUdpSimulationNetwork : ModbusSimulationNetwork
{
    private readonly UdpClient _udpClient;
    private readonly IModbusSlaveNetwork _slaveNetWorks;

    public ModbusUdpSimulationNetwork(
            IpTransportDescriptor ipTransportDescriptor,
            IModbusFactory modbusFactory,
            ModbusSimulatedSlaveFactory modbusSlaveSimulatorFactory,
            ILogger<ModbusUdpSimulationNetwork>? logger = null
        ) : base(modbusSlaveSimulatorFactory, logger)
    {
        _udpClient = new(ipTransportDescriptor.IPEndPoint);
        _slaveNetWorks = modbusFactory.CreateSlaveNetwork(_udpClient);
    }

    public byte UnitId { get; }

    protected override void OnAdded(ModbusSimulationSlave simulated)
    {
        _slaveNetWorks.AddSlave(simulated.Slave);
    }
    protected override void OnRemoved(byte unidId)
    {
        _slaveNetWorks.RemoveSlave(unidId);
    }

    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        await _slaveNetWorks.ListenAsync(cancellationToken);
        await base.OnStartAsync(cancellationToken);
    }
    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();

        _slaveNetWorks.Dispose();
        _udpClient.Dispose();
    }
}