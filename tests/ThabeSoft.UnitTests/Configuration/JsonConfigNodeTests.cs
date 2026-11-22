using System.Text;
using ThabeSoft.Configuration.Json;
using ThabeSoft.Configuration.Nodes;
using ThabeSoft.Configuration.Paths;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.UnitTests.Configuration;


[TestClass]
public class JsonConfigNodeTests
{
    private const string Json = """
    {
        "Ports": [
        {
            "Id": "PortId",
            "Transport": {
                "Type": "Ethernet",
                "ProtocolType": "Tcp",
                "Ip": "127.0.0.1",
                "Port": 2333
            },
            "Devices": [
            {
                "Id": "DeviceId",
                "Protocol": {
                    "Type": "Modbus",
                    "FrameFormat": "Ip"
                }
            }
            ]
        },
        {
            "Id": "SerialPortid",
            "Transport": {
                "Type": "Serial",
                "PortName": "Com3",
                "BaudRate": 9600
            }
        }
        ]
    }
    """;


    private IConfigNode _root = null!;


    [TestInitialize]
    public async Task Setup()
    {
        var parser = new JsonConfigNodeParser(new ConfigPathEvaluator(new ConfigPathParser()));
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(Json));

        _root = await parser.ParseAsync(stream, TestContext.CancellationToken);
    }


    [TestMethod]
    public void GetByPath_ReadsObjectAndValue()
    {
        Assert.AreEqual("PortId", _root.GetStringByPath("Ports[0].Id"));
        Assert.AreEqual("Ethernet", _root.GetStringByPath("Ports[0].Transport.Type"));
        Assert.AreEqual("2333", _root.GetStringByPath("Ports[0].Transport.Port"));
        Assert.AreEqual("Serial", _root.GetStringByPath("Ports[1].Transport.Type"));
    }

    [TestMethod]
    public void GetByPath_SupportsRootAndParent()
    {
        var device = _root.GetByPath("Ports[0].Devices[0]").Required();

        Assert.AreEqual("DeviceId", device.GetStringByPath("Id"));

        // $ 回溯根节点
        Assert.AreEqual("SerialPortid", device.GetStringByPath("$Ports[1].Id"));

        // .. 回溯父节点（Devices 数组），再上一级到 Ports[0]
        Assert.AreEqual("PortId", device.GetByPath("..")!.Parent!.GetStringByPath("Id"));
    }

    [TestMethod]
    public void GetByPath_MissingPathReturnsNull()
    {
        Assert.IsNull(_root.GetByPath("Ports[9].Id"));
        Assert.IsNull(_root.GetByPath("Ports[0].NotExists"));
    }

    [TestMethod]
    public void AsArrayAndAsValue_ReportNodeType()
    {
        Assert.IsNotNull(_root.GetByPath("Ports").AsArray());
        Assert.IsNotNull(_root.GetByPath("Ports[0].Id").AsValue());
        Assert.IsNull(_root.GetByPath("Ports[0]").AsValue());
    }


    public TestContext TestContext { get; set; } = null!;
}
