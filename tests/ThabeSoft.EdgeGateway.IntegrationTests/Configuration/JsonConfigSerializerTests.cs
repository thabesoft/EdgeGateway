using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.DependencyInjection;

namespace ThabeSoft.EdgeGateway.Configuration;


[TestClass]
public class JsonConfigSerializerTests
{
    [TestMethod]
    public async Task A()
    {
        var app = Host.CreateDefaultBuilder()
            .ConfigureServices(x => x.AddEdgeGatewayJsonConfiguration())
            .Build();

        var serializer = app.Services.GetRequiredService<IConfigSerializer>();
        var config = await serializer.DeserializeStringAsync(ConfigJsonString, TestContext.CancellationToken);

        Assert.AreEqual(2, config.Ports.Length);
    }

    public static string ConfigJsonString = """
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
                "FrameFormat": "Ip",
                "TransportType": "Ethernet"
              },
              "Subscriptions": [
                {
                  "Id": "SubscriptionId",
                  "Trigger": {
                    "Type": "Interval",
                    "Interval": "00:00:00.100"
                  },
                  "Tag": {
                    "Kind": "Coil",
                    "Type": "bool",
                    "SlaveId": 1,
                    "Address": 0,
                    "BitOffset": 0
                  }
                },
                {
                  "Id": "SubscriptionId",
                  "Trigger": {
                    "Type": "Interval",
                    "Interval": "00:00:00.100"
                  },
                  "Tag": {
                    "Kind": "Coil",
                    "Type": "Bits",
                    "SlaveId": 1,
                    "Address": 0,
                    "Quantity": 10
                  }
                },
                {
                  "Id": "SubscriptionId",
                  "Trigger": {
                    "Type": "Interval",
                    "Interval": "00:00:00.100"
                  },
                  "Tag": {
                    "Kind": "HoldingRegister",
                    "Type": "Int16",
                    "SlaveId": 1,
                    "Address": 0,
                    "Endianess": "Big"
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

    public TestContext TestContext { get; set; }
}
