using NModbus.IO;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.IO;


internal sealed class NModbusStreamAdapter(IByteDuplex byteChannel) : IStreamResource
{
    public int InfiniteTimeout { get; set; } = -1;
    public int ReadTimeout { get; set; } = 3000;
    public int WriteTimeout { get; set; } = 3000;
    private SemaphoreSlim _lock = new(1, 1);


    public int Read(byte[] buffer, int offset, int count)
    {
        _lock.Wait();

        try
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
        finally
        {
            _lock.Release();
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