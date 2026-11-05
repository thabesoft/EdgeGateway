using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;

namespace ThabeSoft.EdgeGateway.Tags;


public sealed class TagDescriptor<T>
     where T : notnull
{
    internal TagDescriptor() { }


    public required TagId Id { get; init; }
    public required IDataAddress Address { get; init; }
    public required ICodec<T> Codec { get; init; }
}

public static class TagDescriptor
{
    public static TagDescriptor<T> Create<T>(TagId id, IDataAddress address, ICodec<T> codec)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(codec);

        return new()
        {
            Id = id,
            Address = address,
            Codec = codec
        };
    }
}
