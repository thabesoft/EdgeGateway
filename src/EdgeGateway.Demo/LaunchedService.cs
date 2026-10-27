using EdgeGateway.Byte.Codecs;
using EdgeGateway.Channel;
using EdgeGateway.Device;
using EdgeGateway.Modbus.Tag;
using Microsoft.Extensions.Hosting;

namespace EdgeGateway;


internal sealed class LaunchedService(IGateway gateway) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var tag = ModbusTag.HoldingRegister(0x01, 1, QWordCodec.LittleEndian);

        var device = gateway.Ports
            .Where(x => x.Value.ConnectionType == ConnectionType.Serial)
            .SelectMany(x => x.Value.Devices.Values)
            .FirstOrDefault(x => x.ProtocolType == ProtocolType.Modbus);


        var value = await device.ReadAsync(tag);
        Console.WriteLine(value);
        await device.WriteAsync(tag, value + 1);

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}