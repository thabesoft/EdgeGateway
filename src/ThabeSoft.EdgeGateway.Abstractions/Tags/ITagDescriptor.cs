using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;

namespace ThabeSoft.EdgeGateway.Tags;


public interface ITagDescriptor<T> : ITagDescriptor
    where T : notnull
{
    new ICodec<T> Codec { get; }

#if NETCOREAPP
    ICodec ITagDescriptor.Codec => Codec;
#endif
}

public interface ITagDescriptor
{
    TagName Name { get; }
    IDataAddress Address { get; }
    ICodec Codec { get; }
}


public sealed class TagDescriptor<T> : ITagDescriptor<T>
     where T : notnull
{
    internal TagDescriptor() { }


    public required TagName Name { get; init; }
    public required IDataAddress Address { get; init; }
    public required ICodec<T> Codec { get; init; }
    ICodec ITagDescriptor.Codec => Codec;
}

public static class TagDescriptor
{
    public static TagDescriptor<T> Create<T>(TagName tagName, IDataAddress address, ICodec<T> codec)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(tagName);
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(codec);

        return new()
        {
            Name = tagName,
            Address = address,
            Codec = codec
        };
    }
}