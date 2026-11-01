using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NModbus;
using System.Net;
using System.Net.Sockets;

namespace ModbusSlaveSimulator;


public interface IModbusIpSimulator : IModbusSimulator
{
    IPEndPoint IPEndPoint { get; }
}


internal sealed class ModbusIpSimulator(
        IModbusFactory modbusFactory,
        IOptions<ModbusIpSimulatorOptions> options,
        ILogger<ModbusIpSimulator> logger
    ) : ModbusSimulatorBase(modbusFactory, options.Value.UnitId, logger), IModbusIpSimulator
{
    private TcpListener? _tcpListener;
    public IPEndPoint IPEndPoint => (IPEndPoint)_tcpListener!.LocalEndpoint;

    protected override IModbusSlaveNetwork GetModbusSlaveNetwork(IModbusFactory modbusFactory)
    {
        _tcpListener = new(options.Value.IPEndPoint);
        return modbusFactory.CreateSlaveNetwork(_tcpListener);
    }
    protected override ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        _tcpListener?.Start();
        return ValueTask.CompletedTask;
    }
    protected override ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        _tcpListener?.Stop();
        return ValueTask.CompletedTask;
    }
    protected override ValueTask OnDisposeAsync()
    {
        _tcpListener?.Dispose();
        return ValueTask.CompletedTask;
    }
}
