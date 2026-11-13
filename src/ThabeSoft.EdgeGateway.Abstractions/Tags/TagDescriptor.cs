using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;

namespace ThabeSoft.EdgeGateway.Tags;


public sealed class TagDescriptor<T> : IEquatable<TagDescriptor<T>>
     where T : notnull
{
    public TagDescriptor(IDataAddress address, ICodec<T> codec)
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


    public IDataAddress Address { get; }
    public ICodec<T> Codec { get; }


    public bool Equals(TagDescriptor<T> other)
    {
        return Address.Equals(other.Address) && Codec.Equals(other.Codec);
    }

    public override string ToString() => $"Tag({Address}, {Codec})";
}

public static class TagDescriptor
{
    public static TagDescriptor<T> Create<T>(IDataAddress address, ICodec<T> codec) where T : notnull
        => new(address, codec);
}
