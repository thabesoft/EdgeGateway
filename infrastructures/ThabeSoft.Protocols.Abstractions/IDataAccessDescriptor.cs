using ThabeSoft.Codecs;

namespace ThabeSoft.Protocols;


/// <summary>
/// 表示一个数据访问描述符
/// </summary>
public interface IDataAccessDescriptor
{
    /// <summary>
    /// 协议地址
    /// </summary>
    ProtocolAddress Address { get; }

    /// <summary>
    /// 编码器
    /// </summary>
    CodecDescriptor Codec { get; }
}


/// <summary>
/// 表示一个数据访问描述符
/// </summary>
/// <typeparam name="T">数据在C#中的类型</typeparam>
public sealed record class DataAccessDescriptor<T> : IDataAccessDescriptor
     where T : notnull
{
    public DataAccessDescriptor(ProtocolAddress address, CodecDescriptor<T> codec)
    {
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(codec);


        if (address is IRequiredBits addr && codec is IRequiredBits codecRb && codecRb.RequiredBits != addr.RequiredBits)
        {
            throw new InvalidOperationException(
                $"数据位数不匹配, 地址要求读取 {addr.RequiredBits} Bit，但 Codec 要求 {codecRb.RequiredBits} Bit");
        }

        Address = address;
        Codec = codec;
    }

    /// <summary>
    /// 协议地址
    /// </summary>
    public ProtocolAddress Address { get; }

    /// <summary>
    /// 编码器
    /// </summary>
    public CodecDescriptor<T> Codec { get; }

    CodecDescriptor IDataAccessDescriptor.Codec => Codec;

}

public abstract record class DataAccessDescriptor
{
    public static DataAccessDescriptor<T> Create<T>(ProtocolAddress address, CodecDescriptor<T> codec) where T : notnull
        => new(address, codec);
}