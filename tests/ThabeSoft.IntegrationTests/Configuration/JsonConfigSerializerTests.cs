using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ThabeSoft.Codecs.DependencyInjection;
using ThabeSoft.Configuration;
using ThabeSoft.Configuration.DependencyInjection;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.DependencyInjection;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Protocols.DependencyInjection;
using ThabeSoft.Protocols.Modbus;
using ThabeSoft.Protocols.Modbus.DependencyInjection;
using ThabeSoft.Transports.Ethernet;
using ThabeSoft.Transports.Ethernet.DependencyInjection;
using ThabeSoft.Transports.Serial;
using ThabeSoft.Transports.Serial.DependencyInjection;
using ThabeSoft.Triggers.DependencyInjection;

namespace ThabeSoft.EdgeGateway.IntegrationTests.Configuration;


/// <summary>
/// 端到端校验「JSON 文本 → GatewayConfig」的映射链路：
/// 序列化器 + 各配置映射器（传输/协议/编码器/触发器/地址）协同工作。
/// </summary>
[TestClass]
public class JsonConfigSerializerTests
{
    private const string ConfigJson = """
    {
      "Ports": [
        {
          "Id": "PortId",
          "Transport": {
            "Type": "Ethernet",
            "ProtocolType": "Tcp",
            "Ip": "127.0.0.1",
            "Port": 2333,
            "Retry": {
              "StartInterval": "00:00:03",
              "MaxInterval": "00:00:30",
              "BackoffMultiplier": 1.5
            }
          },
          "Devices": [
            {
              "Id": "DeviceId",
              "Protocol": {
                "Type": "Modbus",
                "FrameFormat": "Ip"
              },
              "Subscriptions": [
                {
                  "Id": "SubscriptionId",
                  "Trigger": {
                    "Type": "Interval",
                    "Interval": "00:00:00.100"
                  },
                  "Codec": {
                    "DataType": "Int32",
                    "Layout": "Abcd"
                  },
                  "DataAddress": {
                    "Kind": "HoldingRegister",
                    "SlaveId": 1,
                    "Address": 0,
                    "Quantity": 2
                  }
                },
                {
                  "Id": "SubscriptionId2",
                  "Trigger": {
                    "Type": "Interval",
                    "Interval": "00:00:00.100"
                  },
                  "Codec": {
                    "DataType": "UInt16",
                    "Endianess": "Big"
                  },
                  "DataAddress": {
                    "Kind": "HoldingRegister",
                    "SlaveId": 1,
                    "Address": 2,
                    "Quantity": 1
                  }
                }
              ]
            }
          ]
        },
        {
          "Id": "SerialPortid",
          "Transport": {
            "Type": "Serial",
            "PortName": "Com3",
            "BaudRate": 9600,
            "DataBits": 8,
            "Parity": "None",
            "StopBits": "One",
            "Retry": {
              "StartInterval": "00:00:03",
              "MaxInterval": "00:00:30",
              "BackoffMultiplier": 1.5
            }
          }
        }
      ]
    }
    """;


    [TestMethod]
    public async Task Deserialize_MapsGatewayStructure()
    {
        var app = Host.CreateDefaultBuilder()
            .ConfigureServices(x => x
                .AddLogging()
                .AddConfigPaths()
                .AddJsonConfigNodeParse()
                .AddConfigSerializer()
                .AddEdgeGatewayConfig()
                .AddEthernetTransportConfig()
                .AddSerialTransportConfig()
                .AddCodecsConfig()
                .AddTriggersConfig()
                .AddProtocols()
                .AddModbusProtocolConfig())
            .Build();

        var serializer = app.Services.GetRequiredService<IConfigSerializer>();
        var config = await serializer.DeserializeStringAsync<GatewayConfig>(ConfigJson, TestContext.CancellationToken);

        Assert.AreEqual(2, config.Ports.Length);

        // 以太网端口
        var ethernetPort = config.Ports[0].Descriptor;
        Assert.AreEqual("PortId", ethernetPort.Id.ToString());

        var ip = Assert.IsInstanceOfType<IpTransportDescriptor>(ethernetPort.Transport);
        Assert.AreEqual(2333, ip.Port);

        // 设备与协议
        var device = config.Ports[0].Devices.Single().Descriptor;
        Assert.AreEqual("DeviceId", device.Id.ToString());
        Assert.IsNotNull(device.Protocol);

        // 订阅：Int32 编码器 + HoldingRegister 地址
        var subscriptions = config.Ports[0].Devices.Single().Subscriptions;
        Assert.AreEqual(2, subscriptions.Length);

        var subscription = subscriptions[0].Descriptor;
        Assert.AreEqual("SubscriptionId", subscription.Id.ToString());

        var typed = Assert.IsInstanceOfType<SubscriptionDescriptor<int>>(subscription);
        var address = Assert.IsInstanceOfType<ModbusProtocolAddress>(typed.Tag.Address);
        Assert.AreEqual((ushort)2, address.Quantity);

        // 非 Int32 类型同样能被映射（CodecDescriptorConfigMapper.CanMap 曾只认 Int32/Float）
        Assert.IsInstanceOfType<SubscriptionDescriptor<ushort>>(subscriptions[1].Descriptor);

        // 串口端口
        var serial = Assert.IsInstanceOfType<SerialTransportDescriptor>(config.Ports[1].Descriptor.Transport);
        Assert.AreEqual(9600, serial.BaudRate);
    }

    public TestContext TestContext { get; set; }
}
