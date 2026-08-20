using EdgeGateway.Devices;
using EdgeGateway.Devices.Modbus;
using EdgeGateway.Ports;
using EdgeGateway.Ports.Ethernet;
using EdgeGateway.Ports.SerialPort;
using EdgeGateway.Protocols;
using EdgeGateway.Tags;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace EdgeGateway.Console.Services;

internal sealed class CommandProcesserHostedService(IAppScope appScope, ILogger<CommandProcesserHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var service = appScope.ServiceProvider;

        var tagReader = service.GetRequiredService<ITagReader>();
        var tagWriter = service.GetRequiredService<ITagWriter>();

        var portManager = service.GetRequiredService<IPortManager>();
        var portFactoryManager = service.GetRequiredService<IPortFactoryManager>();


        var deviceFactoryManager = service.GetRequiredService<IDeviceFactoryManager>();


        portFactoryManager.Register(SerialPortFactory.Instance);
        portFactoryManager.Register(new EthernetPortFactory(appScope.ServiceProvider.GetRequiredService<ILoggerFactory>()));

        deviceFactoryManager.Register(ModbusDeviceFactory.Instance);


        var result = await AddPortAsync(portManager, SerialPortConfig.Create("COM2"), stoppingToken);
        if (result.IsFailed) logger.LogError("端口添加失败: {msg}", result);


        var port = portManager.Ports.Values.First();
        var device = port.Devices.Values.First();
        var tag = Tag.Create<int>(port.Address, device.Id, ModbusAddress.HoldingRegisters(1, 1, (ushort)Unsafe.SizeOf<int>()));

        while (true)
        {
            var readResult = await tagReader.ReadAsync(tag);
            System.Console.WriteLine(readResult.Value);

            await Task.Delay(100);
            //var input = System.Console.ReadLine()?.Trim();
            //if (string.IsNullOrWhiteSpace(input)) continue;



            //if(input == "R")
            //{
            //    tagReader.ReadAsync()
            //}
        }


        
    }


    private static async ValueTask<Result> AddPortAsync(IPortManager portManager, IPortConfig portConfig, CancellationToken cancellationToken)
    {
        var addPortResult = portManager.Add(portConfig);
        if (addPortResult.IsFailed) return Result.Fail("端口添加失败").WithErrors(addPortResult.Errors);
        var port = addPortResult.Value;

        var openResult = await addPortResult.Value.OpenAsync(cancellationToken);
        if (openResult.IsFailed) return Result.Fail("端口打开失败").WithErrors(openResult.Errors);

        var device = DeviceConfig.Create(ProtocolType.ModbusRtu, port.Address);
        var addResult = port.AddDevice(device);
        if (addResult.IsFailed) return Result.Fail("设备添加失败").WithErrors(addResult.Errors);

        var connectResult = await addResult.Value.ConnectAsync(cancellationToken);
        if (addResult.IsFailed) return Result.Fail("设备连接失败").WithErrors(addResult.Errors);

        return Result.Ok();
    }
}