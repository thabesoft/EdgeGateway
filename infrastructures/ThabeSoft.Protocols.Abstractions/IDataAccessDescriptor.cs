using ThabeSoft.Codecs;

namespace ThabeSoft.Protocols;



public interface IDataAccessDescriptor
{
    ProtocolAddress Address { get; }
    CodecDescriptor Codec { get; }
}


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


    public CodecDescriptor<T> Codec { get; }
    public ProtocolAddress Address { get; }

    CodecDescriptor IDataAccessDescriptor.Codec => Codec;

}

public abstract record class DataAccessDescriptor
{
    public static DataAccessDescriptor<T> Create<T>(ProtocolAddress address, CodecDescriptor<T> codec) where T : notnull
        => new(address, codec);
}