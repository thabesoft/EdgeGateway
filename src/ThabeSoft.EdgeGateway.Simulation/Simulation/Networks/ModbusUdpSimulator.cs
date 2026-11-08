using NModbus;
using System.Net.Sockets;
using ThabeSoft.EdgeGateway.Simulation.Networks.Slaves;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Simulation.Networks;


internal sealed class ModbusUdpSimulator : ModbusSimulationNetwork
{
    private readonly UdpClient _udpClient;
    private readonly IModbusSlaveNetwork _modbusSlaveNetwork;

    public ModbusUdpSimulator(
            IpTransportDescriptor ipTransportDescriptor,
            IModbusFactory modbusFactory,
            ModbusSimulatedSlaveFactory modbusSlaveSimulatorFactory
        ) : base(modbusSlaveSimulatorFactory)
    {
        _udpClient = new(ipTransportDescriptor.IPEndPoint);
        _modbusSlaveNetwork = modbusFactory.CreateSlaveNetwork(_udpClient);
    }

    public byte UnitId { get; }


    public override IModbusSimulatedSlave AddSlave(byte unitId)
    {
        var item = base.AddSlave(unitId);
        _modbusSlaveNetwork.AddSlave(item.Slave);

        return item;
    }
    public override async Task RemoveSlave(byte unitId)
    {
        await base.RemoveSlave(unitId);
        _modbusSlaveNetwork.RemoveSlave(unitId);
    }


    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        await _modbusSlaveNetwork.ListenAsync(cancellationToken);
        await base.OnStartAsync(cancellationToken);
    }
    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();

        _modbusSlaveNetwork.Dispose();
        _udpClient.Dispose();
    }
}