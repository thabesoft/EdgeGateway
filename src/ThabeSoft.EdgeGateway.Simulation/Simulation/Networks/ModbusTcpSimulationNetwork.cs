using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
    private readonly IpTransportDescriptor _ipTransportDescriptor;
    private readonly ILogger<ModbusTcpSimulationNetwork> _logger;

    public ModbusTcpSimulationNetwork(
            IpTransportDescriptor ipTransportDescriptor,
            IModbusFactory modbusFactory,
            ModbusSimulatedSlaveFactory slaveSimulatorFactory,
            ILogger<ModbusTcpSimulationNetwork>? logger = null
        ) : base(slaveSimulatorFactory, logger)
    {
        _tcpListener = new(ipTransportDescriptor.IPEndPoint);
        _slaveNetWorks = modbusFactory.CreateSlaveNetwork(_tcpListener);
        _ipTransportDescriptor = ipTransportDescriptor;
        _logger = logger ?? NullLogger<ModbusTcpSimulationNetwork>.Instance;
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
        _tcpListener.Start();

        _listenTask = Task.Run(() =>
        {
            _logger.LogInformation("Tcp从站网络开始监听: {Address}", _ipTransportDescriptor.Endpoint);
            _slaveNetWorks.ListenAsync(cancellationToken);

        }, cancellationToken);

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