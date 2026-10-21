using EdgeGateway;
using EdgeGateway.Bytes.Codecs;
using EdgeGateway.Channels;
using EdgeGateway.Devices;
using EdgeGateway.Ports;
using EdgeGateway.Tags;
using NModbus;
using System.Net;

Console.WriteLine("Hello, World!");


var portFactory = new PortFactory(
    channelFactories: [new SerialChannelFactory(), new TcpChannelFactory(), new UdpChannelFactory()],
    deviceFactories: [new ModbusDeviceFactory(new ModbusFactory())]);
await using var gateway = new Gateway(portFactory);

var serialPort = gateway.AddPort(SerialChannelDescriptor.Create("Com2"));
var tcpPort = gateway.AddPort(IpChannelDescriptor.Tcp(IPAddress.Parse("192.168.1.12"), 80));
var udpPort = gateway.AddPort(IpChannelDescriptor.Udp(IPAddress.Parse("192.168.1.12"), 80));

var device1 = serialPort.AddDevice(ModbusDeviceDescriptor.Rtu);
var device2 = tcpPort.AddDevice(ModbusDeviceDescriptor.Tcp);
var device3 = udpPort.AddDevice(ModbusDeviceDescriptor.Udp);

await gateway.StartAsync();

var tag = ModbusTag.HoldingRegister(0x01, 1, QWordCodec.LittleEndian);

var value = await device1.ReadAsync(tag);
Console.WriteLine(value);
await device1.WriteAsync(tag, value + 1);