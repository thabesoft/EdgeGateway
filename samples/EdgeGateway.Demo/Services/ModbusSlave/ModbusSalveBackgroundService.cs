using Microsoft.Extensions.Hosting;
using NModbus;
using NModbus.Serial;
using System.IO.Ports;

namespace EdgeGateway.Services.ModbusSlave;


internal sealed class ModbusSalveBackgroundService : IHostedService
{
    private readonly SerialPort _serialPort;
    private readonly IModbusSlaveNetwork _slaveNetwork;
    private CancellationTokenSource _slaveNetworkCts = new();
    private MousePositionUpdater _mousePositionUpdater;


    public ModbusSalveBackgroundService(IModbusFactory modbusFactory)
    {
        _serialPort = new SerialPort("COM2");
        var serialPortAdapter = new SerialPortAdapter(_serialPort);
        _slaveNetwork = modbusFactory.CreateRtuSlaveNetwork(serialPortAdapter);


        var slave1 = modbusFactory.CreateSlave(1);
        _slaveNetwork.AddSlave(slave1);
        _mousePositionUpdater = new MousePositionUpdater(slave1.DataStore);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _serialPort.Open();
        await _mousePositionUpdater.StartAsync(cancellationToken);

        _ = Task.Run(async () => await _slaveNetwork.ListenAsync(_slaveNetworkCts.Token), cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _mousePositionUpdater.StopAsync(cancellationToken);
        await _slaveNetworkCts.CancelAsync();
        _slaveNetwork.Dispose();
    }
}
