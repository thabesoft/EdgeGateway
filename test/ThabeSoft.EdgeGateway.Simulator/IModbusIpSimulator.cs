using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NModbus;
using System.Net;
using System.Net.Sockets;

namespace ThabeSoft.EdgeGateway.Simulator;


public interface IModbusIpSimulator : IModbusSimulator
{
    IPEndPoint IPEndPoint { get; }
}


internal sealed class ModbusIpSimulator(
        IModbusFactory modbusFactory,
        IOptions<ModbusIpSimulatorOptions> options,
        ILogger<ModbusIpSimulator> logger
    ) : ModbusSimulator(modbusFactory, options.Value.UnitId, logger), IModbusIpSimulator
{
    private TcpListener? _tcpListener;

    public IPEndPoint IPEndPoint => (IPEndPoint)_tcpListener!.LocalEndpoint;

    protected override IModbusSlaveNetwork GetModbusSlaveNetwork(IModbusFactory modbusFactory)
    {
        _tcpListener = new(options.Value.IPEndPoint);
        return modbusFactory.CreateSlaveNetwork(_tcpListener);
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _tcpListener?.Start();
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _tcpListener?.Stop();
        await base.StopAsync(cancellationToken);
    }
    public override ValueTask DisposeAsync()
    {
        _tcpListener?.Dispose();
        return base.DisposeAsync();
    }
}
