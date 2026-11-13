using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NModbus;
using System.Net.Sockets;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Devices.Modbus;
using ThabeSoft.EdgeGateway.Slaves.Modbus;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports.Modbus;


internal sealed class ModbusTcpSimulationPort : SimulationPortBase
{
    private readonly TcpListener _tcpListener;
    private Task? _listenTask;
    private readonly IModbusTcpSlaveNetwork _slaveNetWork;
    private readonly IpTransportDescriptor _ipTransportDescriptor;
    private readonly ILogger<ModbusTcpSimulationPort> _logger;

    public ModbusTcpSimulationPort(
            IpTransportDescriptor ipTransportDescriptor,
            IModbusFactory modbusFactory,
            SimulationDeviceFactorySelector deviceFactorySelector,
            ILogger<ModbusTcpSimulationPort>? logger = null
        ) : base(ipTransportDescriptor.Endpoint, deviceFactorySelector, logger)
    {
        _tcpListener = new(ipTransportDescriptor.IPEndPoint);
        _slaveNetWork = modbusFactory.CreateSlaveNetwork(_tcpListener);
        _ipTransportDescriptor = ipTransportDescriptor;
        _logger = logger ?? NullLogger<ModbusTcpSimulationPort>.Instance;
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
        _tcpListener.Start();

        _listenTask = Task.Run(() =>
        {
            _logger.LogInformation("Tcp端口开始监听: {Address}", _ipTransportDescriptor.Endpoint);
            _slaveNetWork.ListenAsync(cancellationToken);

        }, cancellationToken);

        await base.OnStartAsync(cancellationToken);
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        await base.OnStopAsync(cancellationToken);
        _tcpListener.Stop();

        _logger.LogInformation("Tcp端口停止监听: {Address}", _ipTransportDescriptor.Endpoint);
    }
    protected override async ValueTask OnDisposeAsync()
    {
        await base.OnDisposeAsync();

        _slaveNetWork.Dispose();

#if NETSTANDARD2_0
        _tcpListener?.Stop();
#else

        _tcpListener?.Dispose();
#endif

        _logger.LogInformation("Tcp端口释放: {Address}", _ipTransportDescriptor.Endpoint);
    }
}