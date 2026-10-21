namespace EdgeGateway.Channel;


public interface IChannel : IByteReader, IByteWriter, IAsyncDisposable
{
    bool IsOpen { get; }

    ValueTask OpenAsync(CancellationToken ct = default);
    ValueTask CloseAsync(CancellationToken ct = default);
}

public interface IByteReader
{
    /// <summary>
    /// 读取指定缓冲区大小的字节
    /// </summary>
    ValueTask ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
}

public interface IByteWriter
{
    /// <summary>
    /// 写入全部字节
    /// </summary>
    ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);
}

public interface IBufferDiscardable
{
    void DiscardInBuffer();
}





public sealed class StreamByteWriter : IByteWriter
{
    private readonly Stream _stream;
    private StreamByteWriter(Stream stream) => _stream = stream;

    public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        return _stream.WriteAsync(data, cancellationToken);
    }


    public static StreamByteWriter Create(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanWrite) throw new ArgumentException("Stream is not writable.", nameof(stream));

        return new StreamByteWriter(stream);
    }
}

public sealed class StreamByteReader : IByteReader
{
    private readonly Stream _stream;
    private StreamByteReader(Stream stream) => _stream = stream;

    public ValueTask ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _stream.ReadExactlyAsync(buffer, cancellationToken);
    }


    public static StreamByteReader Create(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead) throw new ArgumentException("Stream is not readable.", nameof(stream));

        return new StreamByteReader(stream);
    }
}