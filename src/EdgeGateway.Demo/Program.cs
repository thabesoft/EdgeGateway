using EdgeGateway;
using EdgeGateway.Byte.Codecs;
using EdgeGateway.Ethernet;
using EdgeGateway.Modbus;
using EdgeGateway.Modbus.Tag;
using EdgeGateway.Port;
using EdgeGateway.Serial;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

Console.WriteLine("Hello, World!");


var app = Host.CreateDefaultBuilder(args)
    .ConfigureServices(x => x
        .AddEdgeGateway()
        .AddSerial()
        .AddEthernet()
        .AddModbus()
        )
    .Build();

var services = app.Services;
var gateway = services.GetRequiredService<IGateway>();



var serialPort = gateway.AddPort(ChannelPortDescriptor.From(SerialChannelDescriptor.Create("Com2")));
var tcpPort = gateway.AddPort(ChannelPortDescriptor.From(IpChannelDescriptor.Tcp(IPAddress.Parse("192.168.1.12"), 80)));
var udpPort = gateway.AddPort(ChannelPortDescriptor.From(IpChannelDescriptor.Udp(IPAddress.Parse("192.168.1.12"), 80)));

var device1 = serialPort.AddDevice(ModbusDeviceDescriptor.Rtu);
var device2 = tcpPort.AddDevice(ModbusDeviceDescriptor.Tcp);
var device3 = udpPort.AddDevice(ModbusDeviceDescriptor.Udp);

await gateway.StartAsync();

var tag = ModbusTag.HoldingRegister(0x01, 1, QWordCodec.LittleEndian);

var value = await device1.ReadAsync(tag);
Console.WriteLine(value);
await device1.WriteAsync(tag, value + 1);