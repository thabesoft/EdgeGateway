using NModbus.IO;

namespace ThabeSoft.EdgeGateway.IO;

internal sealed class NModbusStreamAdapter(IByteStream byteChannel) : IStreamResource
{
    public int InfiniteTimeout { get; set; } = 1000;
    public int ReadTimeout { get; set; } = 1000;
    public int WriteTimeout { get; set; } = 1000;


    public int Read(byte[] buffer, int offset, int count)
    {
        var targetSlice = buffer.AsMemory(offset, count);

        using var cts = new CancellationTokenSource(ReadTimeout);
        try
        {
            return byteChannel.ReadAsync(targetSlice, cts.Token).AsTask().GetAwaiter().GetResult();
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
            throw new TimeoutException($"Read timed out after {ReadTimeout} ms.");
        }
    }

    public void Write(byte[] buffer, int offset, int count)
    {
        var targetSlice = buffer.AsMemory(offset, count);

        using var cts = new CancellationTokenSource(WriteTimeout);
        try
        {
            byteChannel.WriteAsync(targetSlice, cts.Token).AsTask().GetAwaiter().GetResult();
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
            throw new TimeoutException($"Write timed out after {WriteTimeout} ms.");
        }
    }

    public void DiscardInBuffer()
    {
        (byteChannel as IReadBufferDiscardable)?.DiscardInputBuffer();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}