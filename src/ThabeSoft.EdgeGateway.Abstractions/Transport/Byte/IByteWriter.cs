namespace ThabeSoft.EdgeGateway.Transport.Byte;

public interface IByteWriter
{
    ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);
}
