using System.Collections;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


public static class Modbus
{
    #region --协议--

    public static ModbusProtocolDescriptor Rtu { get; } = new()
    {
        Connection = ConnectionType.Serial,
        FrameFormat = ModbusFrameFormat.Rtu
    };
    public static ModbusProtocolDescriptor Ascii { get; } = new()
    {
        Connection = ConnectionType.Serial,
        FrameFormat = ModbusFrameFormat.Ascii
    };
    public static ModbusProtocolDescriptor Ip { get; } = new()
    {
        Connection = ConnectionType.Ethernet,
        FrameFormat = ModbusFrameFormat.Ip
    };

    #endregion

    #region --标签--

    public static TagDescriptor<bool> Coil(byte slaveId, ushort address, int bitOffset = 0) =>
        TagDescriptor.Create(ModbusDataAddress.Coil(slaveId, address, 1), Codec.Bool(bitOffset));
    public static TagDescriptor<BitArray> Coils(byte slaveId, ushort address, ushort quantity) =>
        TagDescriptor.Create(ModbusDataAddress.Coil(slaveId, address, quantity), Codec.BitsLsbFirst(quantity));

    public static TagDescriptor<bool> DiscreteInput(byte slaveId, ushort address) =>
        TagDescriptor.Create(ModbusDataAddress.DiscreteInput(slaveId, address, 1), Codec.Bool(0));
    public static TagDescriptor<BitArray> DiscreteInputs(byte slaveId, ushort address, ushort quantity) =>
        TagDescriptor.Create(ModbusDataAddress.DiscreteInput(slaveId, address, quantity), Codec.BitsLsbFirst(quantity));


    public static TagDescriptor<T> HoldingRegister<T>(byte slaveId, ushort address, ICodec<T> codec) where T : unmanaged
    {
        if (codec is not IRequiredBits requiredBits)
            throw new InvalidOperationException("编码器缺少数据位信息");

        // 位数必须能被 16 整除（整数个 word）
        if (requiredBits.RequiredBits <= 0 || requiredBits.RequiredBits % 16 != 0)
            throw new InvalidOperationException("编码器位数必须是 16 的正整数倍");

        var wordCount = requiredBits.RequiredBits / 16;

        const int MaxModbusWords = 125;
        if (wordCount is < 1 or > MaxModbusWords)
            throw new InvalidOperationException($"编码器需要 {wordCount} 个寄存器，超出 Modbus 范围（1~{MaxModbusWords}）");


        var dataAddress = ModbusDataAddress.HoldingRegister(slaveId, address, (ushort)wordCount);
        return TagDescriptor.Create(dataAddress, codec);
    }

    public static TagDescriptor<T> InputRegister<T>(byte slaveId, ushort address, ICodec<T> codec) where T : unmanaged
    {
        if (codec is not IRequiredBits requiredBits)
            throw new InvalidOperationException("编码器缺少数据位信息");

        // 位数必须能被 16 整除（整数个 word）
        if (requiredBits.RequiredBits <= 0 || requiredBits.RequiredBits % 16 != 0)
            throw new InvalidOperationException("编码器位数必须是 16 的正整数倍");

        var wordCount = requiredBits.RequiredBits / 16;

        const int MaxModbusWords = 125;
        if (wordCount is < 1 or > MaxModbusWords)
            throw new InvalidOperationException($"编码器需要 {wordCount} 个寄存器，超出 Modbus 范围（1~{MaxModbusWords}）");


        var dataAddress = ModbusDataAddress.InputRegister(slaveId, address, (ushort)wordCount);
        return TagDescriptor.Create(dataAddress, codec);
    }

    #endregion
}