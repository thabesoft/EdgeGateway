using EdgeGateway.Byte.Codecs;
using EdgeGateway.Device;
using EdgeGateway.Polling.Triggers;
using EdgeGateway.Tag;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO.Ports;
using ThabeSoft.EdgeGateway.Modbus;

namespace EdgeGateway.Services;


internal sealed class GatewayHostedService(IGateway gateway, ILogger<GatewayHostedService> logger)
    : IHostedService, IAsyncDisposable
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await gateway.StartAsync(cancellationToken);
        logger.LogInformation("网关已启动");

        await OnInit(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await gateway.StopAsync(cancellationToken);
        logger.LogInformation("网关已停止");
    }

    public async ValueTask DisposeAsync()
    {
        await gateway.DisposeAsync();
    }




    private async ValueTask OnInit(CancellationToken cancellationToken)
    {
        var port = gateway.AddSerialPort(x => x
            .PortName("COM2")
            .BaudRate(9600)
            .Parity(Parity.None)
            .DataBits(8)
            .StopBits(StopBits.One));

        var device = port.AddDevice(DeviceDescriptor.Serial(ModbusProtocolDescriptor.Rtu));
        var tagName = TagName.From("温度");
        var tag = device.Register(tagName, ModbusTag.HoldingRegister(1, 0, WordCodec.BigEndian), new FixedIntervalTrigger(TimeSpan.FromSeconds(1)));
        tag.Updated += delegate
        {
            logger.LogInformation("网关标签更新: {v}", tag.Value);
        };

        await port.OpenAsync(cancellationToken);

        //await Task.Delay(1000, cancellationToken);

        //await port.CloseAsync(cancellationToken);


        //// 卸载
        //device.Unregister(tagName);
        //await port.RemoveDeviceAsync(device.Id, cancellationToken);
        //await gateway.RemovePortAsync(port.Id, cancellationToken);
    }
}