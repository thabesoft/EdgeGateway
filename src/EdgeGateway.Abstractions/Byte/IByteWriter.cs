namespace EdgeGateway.Byte;

public interface IByteWriter
{
    ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);
}
