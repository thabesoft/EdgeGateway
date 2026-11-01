namespace ThabeSoft.EdgeGateway.IO;

public interface IByteWriter
{
    ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);
}
