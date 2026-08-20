namespace EdgeGateway.Devices.Modbus;


[TestClass]
public sealed class ModbusAddressTests
{
    [TestMethod]
    [DataRow((byte)0x01, (ushort)1, (ushort)2)]
    [DataRow((byte)0x05, (ushort)100, (ushort)10)]
    public async Task Coils_ToString(byte slaveId, ushort address, ushort quantity) 
        => Console.WriteLine(ModbusAddress.Coils(slaveId, 1, quantity));


    [TestMethod]
    [DataRow((byte)0x01, (ushort)1, (ushort)2)]
    [DataRow((byte)0x05, (ushort)100, (ushort)10)]
    public async Task DiscreteInputs_ToString(byte slaveId, ushort address, ushort quantity)
        => Console.WriteLine(ModbusAddress.DiscreteInputs(slaveId, address, quantity));


    [TestMethod]
    [DataRow((byte)0x01, (ushort)1, (ushort)2)]
    [DataRow((byte)0x05, (ushort)100, (ushort)10)]
    public async Task HoldingRegisters_ToString(byte slaveId, ushort address, ushort quantity)
        => Console.WriteLine(ModbusAddress.HoldingRegisters(slaveId, address, quantity));


    [TestMethod]
    [DataRow((byte)0x01, (ushort)1, (ushort)2)]
    [DataRow((byte)0x05, (ushort)100, (ushort)10)]
    public async Task InputRegisters_ToString(byte slaveId, ushort address, ushort quantity)
        => Console.WriteLine(ModbusAddress.InputRegisters(slaveId, address, quantity));
}