namespace EdgeGateway.Channels;


public interface IChannel : IByteChannel, IAsyncDisposable
{
    ChannelAddress Address { get; }

    ValueTask OpenAsync(CancellationToken cancellationToken = default);
    ValueTask CloseAsync(CancellationToken cancellationToken = default);
}

public interface IByteReader
{
    ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
}

public interface IByteWriter
{
    ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);
}

public interface IByteChannel : IByteReader, IByteWriter;


public interface IInputBufferDiscardable
{
    void DiscardInputBuffer();
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