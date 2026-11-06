using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;

namespace ThabeSoft.EdgeGateway.Tags;


public sealed class TagDescriptor<T>
     where T : notnull
{
    internal TagDescriptor() { }

    public required IDataAddress Address { get; init; }
    public required ICodec<T> Codec { get; init; }
}

public static class TagDescriptor
{
    public static TagDescriptor<T> Create<T>(IDataAddress address, ICodec<T> codec)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(codec);


        if (address is IRequiredBits addr && codec is IRequiredBits codecRb && codecRb.RequiredBits != addr.RequiredBits)
        {
            throw new InvalidOperationException(
                $"数据位数不匹配, 地址要求读取 {addr.RequiredBits} Bit，但 Codec 要求 {codecRb.RequiredBits} Bit");
        }

        return new()
        {
            Address = address,
            Codec = codec
        };
    }
}
