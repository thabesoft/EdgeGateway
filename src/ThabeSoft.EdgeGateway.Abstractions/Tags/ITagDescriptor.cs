using ThabeSoft.Codecs;

namespace ThabeSoft.EdgeGateway.Tags;



public interface ITagDescriptor
{
    DataAddress Address { get; }
    CodecDescriptor Codec { get; }
}


public sealed record class TagDescriptor<T> : ITagDescriptor
     where T : notnull
{
    public TagDescriptor(DataAddress address, CodecDescriptor<T> codec)
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
    public DataAddress Address { get; }

    CodecDescriptor ITagDescriptor.Codec => Codec;

}

public abstract record class TagDescriptor
{
    public static TagDescriptor<T> Create<T>(DataAddress address, CodecDescriptor<T> codec) where T : notnull
        => new(address, codec);
}