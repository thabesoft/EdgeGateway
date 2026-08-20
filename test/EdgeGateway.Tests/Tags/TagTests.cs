using EdgeGateway.Data;
using EdgeGateway.Devices;
using EdgeGateway.Devices.Modbus;
using EdgeGateway.Ports;

namespace EdgeGateway.Tags;


[TestClass]
public sealed class TagTests
{
    [TestMethod]
    [DataRow(nameof(PortType.SerialPort), "COM1", "Plc")]
    public async Task ReadCoils_ToString(string portTypeString, string locationString, string deviceIdString)
    {
        var dataAddress = ModbusAddress.HoldingRegisters(0x02, 200, 2);
        var typeDescriptor = TypeDescriptors.Unmanaged<int>();
        var dataDescriptor = DataDescriptor.Create(dataAddress, typeDescriptor);

        var portType = PortType.Parse(portTypeString);
        var portAddress = PortAddress.Create(portType, locationString);

        var deviceId = DeviceId.FromString(deviceIdString);

        var tag = Tag.Create(portAddress, deviceId, dataDescriptor);
        Console.WriteLine(tag);
    }
}