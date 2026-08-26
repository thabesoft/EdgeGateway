using EdgeGateway.Data;

namespace EdgeGateway.Modbus;


public sealed record class ModbusAddress(ModbusRegion Region, byte SlaveId, ushort Address, ushort Quantity) : IDataAddress
{
    /// <summary>
    /// 返回 Modbus 地址的字符串表示。
    /// </summary>
    /// <returns>
    /// 格式为 <c>Slave {SlaveId} {RegionPrefix}{Address + 1}</c>，例如：
    /// <list type="bullet">
    /// <item><description><c>Slave 1 4x1</c> → 从站 1，保持寄存器地址 40001</description></item>
    /// <item><description><c>Slave 1 0x1</c> → 从站 1，线圈地址 00001</description></item>
    /// <item><description><c>Slave 2 3x10</c> → 从站 2，输入寄存器地址 30010</description></item>
    /// <item><description><c>Slave 1 1x5</c> → 从站 1，离散输入地址 10005</description></item>
    /// </list>
    /// 区域前缀与 PLC 地址的对应关系：
    /// <list type="table">
    /// <item><description><c>0x</c> → 线圈 (Coils)</description></item>
    /// <item><description><c>1x</c> → 离散输入 (DiscreteInputs)</description></item>
    /// <item><description><c>3x</c> → 输入寄存器 (InputRegisters)</description></item>
    /// <item><description><c>4x</c> → 保持寄存器 (HoldingRegisters)</description></item>
    /// </list>
    /// 地址采用 0-based 偏移，显示时自动转换为 1-based PLC 地址。
    /// </returns>
    public override string ToString()
    {
        var prefix = Region switch
        {
            ModbusRegion.Coils => "0x",
            ModbusRegion.DiscreteInputs => "1x",
            ModbusRegion.InputRegisters => "3x",
            ModbusRegion.HoldingRegisters => "4x",
            _ => ""
        };
        return $"Slave {SlaveId} {prefix}{Address + 1}";
    }



    public static ModbusAddress Coils(byte slaveId, ushort address, ushort quantity)
        => new(ModbusRegion.Coils, slaveId, address, quantity);
    public static ModbusAddress HoldingRegisters(byte slaveId, ushort address, ushort quantity)
        => new(ModbusRegion.HoldingRegisters, slaveId, address, quantity);

    public static ModbusAddress DiscreteInputs(byte slaveId, ushort address, ushort quantity)
        => new(ModbusRegion.DiscreteInputs, slaveId, address, quantity);
    public static ModbusAddress InputRegisters(byte slaveId, ushort address, ushort quantity)
        => new(ModbusRegion.InputRegisters, slaveId, address, quantity);
}