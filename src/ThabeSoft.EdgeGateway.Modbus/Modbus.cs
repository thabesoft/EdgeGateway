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

    public static ModbusProtocolDescriptor Rtu { get; } =
        new(ModbusFrameFormat.Rtu, ConnectionType.Serial);

    public static ModbusProtocolDescriptor Ascii { get; } =
       new(ModbusFrameFormat.Ascii, ConnectionType.Serial);

    public static ModbusProtocolDescriptor Ip { get; } =
       new(ModbusFrameFormat.Ip, ConnectionType.Ethernet);

    #endregion

    #region --标签--

    public static TagDescriptor<bool> Coil(byte slaveId, ushort address, int bitOffset = 0) =>
        TagDescriptor.Create(ModbusDataAddress.Coil(slaveId, address, 1), Codec.Bool(bitOffset));
    public static TagDescriptor<bool> DiscreteInput(byte slaveId, ushort address) =>
        TagDescriptor.Create(ModbusDataAddress.DiscreteInput(slaveId, address, 1), Codec.Bool(0));

    public static TagDescriptor<BitArray> Coils(byte slaveId, ushort address, ushort quantity)
    {
        ValidateCoilQuantity(quantity);
        return TagDescriptor.Create(ModbusDataAddress.Coil(slaveId, address, quantity), Codec.BitsLsbFirst(quantity));
    }
    public static TagDescriptor<BitArray> DiscreteInputs(byte slaveId, ushort address, ushort quantity)
    {
        ValidateDiscreteInputQuantity(quantity);
        return TagDescriptor.Create(ModbusDataAddress.DiscreteInput(slaveId, address, quantity), Codec.BitsLsbFirst(quantity));
    }


    public static TagDescriptor<T> HoldingRegister<T>(byte slaveId, ushort address, ICodec<T> codec) where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(codec);
        // 位数必须能被 16 整除（整数个 word）
        if (codec.RequiredBits <= 0 || codec.RequiredBits % 16 != 0)
            throw new InvalidOperationException("编码器位数必须是 16 的正整数倍");

        var wordCount = (ushort)codec.RequiredWords;
        ValidateHoldingRegisterQuantity((ushort)codec.RequiredWords);


        var dataAddress = ModbusDataAddress.HoldingRegister(slaveId, address, wordCount);
        return TagDescriptor.Create(dataAddress, codec);
    }

    public static TagDescriptor<T> InputRegister<T>(byte slaveId, ushort address, ICodec<T> codec) where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(codec);

        // 位数必须能被 16 整除（整数个 word）
        if (codec.RequiredBits <= 0 || codec.RequiredBits % 16 != 0)
            throw new InvalidOperationException("编码器位数必须是 16 的正整数倍");


        var wordCount = (ushort)codec.RequiredWords;
        ValidateInputRegisterQuantity(wordCount);


        var dataAddress = ModbusDataAddress.InputRegister(slaveId, address, wordCount);
        return TagDescriptor.Create(dataAddress, codec);
    }

    #endregion




    /// <summary>
    /// 校验线圈数量是否合法。
    /// 读线圈(FC01)协议上限2000，写线圈(FC0F)协议上限1968；
    /// </summary>
    private static void ValidateCoilQuantity(ushort quantity)
    {
        const int MinQuantity = 1;
        const int MaxQuantity = 1968; // 读写取交集，写操作更严格

        if (quantity < MinQuantity || quantity > MaxQuantity)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                $"线圈数量必须在 {MinQuantity}~{MaxQuantity} 之间。"
                + $"（读线圈FC01上限2000，写线圈FC0F上限1968，标签不分读写故取1968）");
        }
    }

    /// <summary>
    /// 校验离散输入数量是否合法。
    /// 离散输入只读(FC02)，协议上限2000，无写操作限制。
    /// </summary>
    private static void ValidateDiscreteInputQuantity(ushort quantity)
    {
        const int MinQuantity = 1;
        const int MaxQuantity = 2000; // 只读，直接使用协议上限

        if (quantity < MinQuantity || quantity > MaxQuantity)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                $"离散输入数量必须在 {MinQuantity}~{MaxQuantity} 之间。");
        }
    }

    /// <summary>
    /// 校验保持寄存器数量是否合法。
    /// 保持寄存器可读可写：读(FC03)上限125，写多个(FC16)上限123；
    /// 标签不分读写，取交集 123 作为上限。
    /// </summary>
    private static void ValidateHoldingRegisterQuantity(ushort quantity)
    {
        const int MinQuantity = 1;
        const int MaxQuantity = 123; // 读写取交集，写操作更严格

        if (quantity < MinQuantity || quantity > MaxQuantity)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                $"保持寄存器数量必须在 {MinQuantity}~{MaxQuantity} 之间。"
                + $"（读FC03上限125，写多个FC16上限123，标签不分读写故取123）");
        }
    }

    /// <summary>
    /// 校验输入寄存器数量是否合法。
    /// 输入寄存器只读(FC04)，协议上限125，无写操作限制。
    /// </summary>
    private static void ValidateInputRegisterQuantity(ushort quantity)
    {
        const int MinQuantity = 1;
        const int MaxQuantity = 125; // 只读，直接使用协议上限

        if (quantity < MinQuantity || quantity > MaxQuantity)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                $"输入寄存器数量必须在 {MinQuantity}~{MaxQuantity} 之间。");
        }
    }
}