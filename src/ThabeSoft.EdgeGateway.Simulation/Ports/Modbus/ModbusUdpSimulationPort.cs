using Microsoft.Extensions.Logging;
using NModbus;
using System.Net.Sockets;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Devices.Modbus;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Networks.Modbus;


internal sealed class ModbusUdpSimulationPort : SimulationPortBase
{
    private readonly UdpClient _udpClient;
    private readonly IModbusSlaveNetwork _slaveNetWork;

    public ModbusUdpSimulationPort(
            IpTransportDescriptor ipTransportDescriptor,
            IModbusFactory modbusFactory,
            SimulationDeviceFactorySelector deviceFactorySelector,
            ILogger<ModbusUdpSimulationPort>? logger = null
        ) : base(ipTransportDescriptor.Endpoint, deviceFactorySelector, logger)
    {
        _udpClient = new(ipTransportDescriptor.IPEndPoint);
        _slaveNetWork = modbusFactory.CreateSlaveNetwork(_udpClient);
    }

    public byte UnitId { get; }

    protected override void OnAdded(ISimulationDevice device)
    {
        if (device is not ModbusSimulationDevice modbus)
            return;

        _slaveNetWork.AddSlave(modbus.Slave);
    }
    protected override void OnRemoved(ISimulationDeviceId deviceId)
    {
        if (deviceId is not ModbusSimulationDeviceId modbus)
            return;

        _slaveNetWork.RemoveSlave(modbus.Value);
    }

    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        await _slaveNetWork.ListenAsync(cancellationToken);
        await base.OnStartAsync(cancellationToken);
    }
    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();

        _slaveNetWork.Dispose();
        _udpClient.Dispose();
    }
}