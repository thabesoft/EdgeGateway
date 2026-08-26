using EdgeGateway.Serialization;
using System.Text;

namespace EdgeGateway.Options.Serialization;


[TestClass]
public class GatewayOptionsSerializerTests
{
    [TestMethod]
    public async Task FuckAsync()
    {
        const string json = """
            {
              "Version": "1.0",
              "Ports": [
                {
                  "Type": "SerialPort",
                  "PortName": "COM2",
                  "BaudRate": 9600,
                  "Parity": "None",
                  "DataBits": 8,
                  "StopBits": 1
                },
                {
                  "Type": "Tcp",
                  "Remote": "192.168.17.233:10502"
                }
              ],
              "Devices": [
                {
                  "Id": "Plc1",
                  "Protocol": "Modbus-Rtu",
                  "PortAddress": {
                    "Type": "SerialPort",
                    "Location": "COM2"
                  },
                  "Tags": [
                    {
                      "Name": "温度",
                      "Enable": true,
                      "Address": {
                        "SlaveId": 1,
                        "FunctionCode": 3,
                        "StartAddress": 1,
                        "Quantity": 2
                      },
                      "Type": "Int32",
                      "ByteOrder": "LittleEndian",
                      "Unit": "°C",
                      "Scale": 0.1,
                      "Offset": -40.0,
                      "AlarmLow": 0.0,
                      "AlarmHigh": 100.0,
                      "ScanIntervalMs": 200
                    }
                  ]
                }
              ],
              "Storage": {
                "Mode": "Local",
                "LocalPath": "data/",
                "MaxCacheSizeMb": 1024,
                "RetentionDays": 30,
                "WriteIntervalMs": 1000,
                "CompressEnabled": true
              },
              "Mqtt": {
                "Enable": true,
                "Broker": "192.168.1.200",
                "Port": 1883,
                "ClientId": "gateway001",
                "Username": "gateway",
                "Password": "password123",
                "UseTls": false,
                "TopicPrefix": "factory/gateway001",
                "PublishIntervalMs": 1000,
                "Qos": 1,
                "Retain": false
              }
            }
            """;

        var bytes = Encoding.UTF8.GetBytes(json);
        using var ms = new MemoryStream(bytes);


        JsonGatewayOptionsSerializer serializer = new();
        var options = await serializer.DeserializeAsync(ms, TestContext.CancellationToken);


    }

    public TestContext TestContext { get; set; }
}
