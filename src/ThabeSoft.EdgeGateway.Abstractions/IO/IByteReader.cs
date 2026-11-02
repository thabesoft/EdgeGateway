namespace ThabeSoft.EdgeGateway.IO;

public interface IByteReader
{
    ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
}

public static class ByteReaderExtensions
{
    extension(IByteReader reader)
    {
        public async ValueTask ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            int filled = 0;

            while (filled < buffer.Length)
            {
                int n = await reader.ReadAsync(buffer[filled..], cancellationToken);
                if (n == 0) throw new EndOfStreamException();
                filled += n;
            }
        }
    }
}