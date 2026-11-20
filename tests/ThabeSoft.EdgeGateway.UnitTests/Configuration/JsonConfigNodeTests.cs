using System.Text;
using System.Text.Json.Nodes;
using ThabeSoft.Configuration;
using ThabeSoft.Configuration.Json.Nodes;
using ThabeSoft.Configuration.Paths;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.UnitTests.Configuration;


[TestClass]
public class JsonConfigNodeTests
{
    private JsonConfigNode Node { get; set; } = null!;

    [TestInitialize]
    public void Setup()
    {
        var json = JsonNode.Parse("""
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
                            "Layout": "ABCD"
                        },
                        "DataAddress": {
                            "Kind": "Coil",
                            "SlaveId": 1,
                            "Address": 0,
                            "Quantity": 0
                        },
                        "Slinks": [
                        "Test"
                        ]
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
        """).Required();

        Node = new JsonConfigNode(json, new ConfigPathEvaluator(new ConfigPathParser()));
    }



    [TestMethod]
    public void TestMethod1()
    {
        var id = Node.GetByPath("Ports[0].Id")?.AsString();
        Assert.AreEqual("PortId", id);

        var protocolTypeNode = Node.GetByPath("Ports[0].Devices[0].Subscriptions[0].DataAddress......Protocol.Type");
        var protocolType = protocolTypeNode?.AsString()
            .Required("缺失协议类型")
            .ToEnum<ProtocolType>();

        Assert.AreEqual(ProtocolType.Modbus, protocolType);


        var transportType = protocolTypeNode?.GetByPath("$Ports[0].Transport.Type")?.AsString()
             .Required("缺少传输类型")
             .ToEnum<TransportType>();

        Assert.AreEqual(TransportType.Ethernet, transportType);
    }
}