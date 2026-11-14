using NModbus.IO;
using System.Diagnostics;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.IO;


internal sealed class NModbusStreamAdapter(IByteDuplex byteDuplex) : IStreamResource
{
    private readonly object _lock = new();
    public int InfiniteTimeout { get; set; } = -1;
    public int ReadTimeout { get; set; } = 3000;
    public int WriteTimeout { get; set; } = 3000;


    public int Read(byte[] buffer, int offset, int count)
    {
        var targetSlice = buffer.AsMemory(offset, count);

        using var cts = new CancellationTokenSource(ReadTimeout);
        try
        {
            return byteDuplex.ReadAsync(targetSlice, cts.Token).AsTask().GetAwaiter().GetResult();
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
            throw new TimeoutException($"Read timed out after {ReadTimeout} ms.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }

    public void Write(byte[] buffer, int offset, int count)
    {
        var targetSlice = buffer.AsMemory(offset, count);

        using var cts = new CancellationTokenSource(WriteTimeout);
        try
        {
            byteDuplex.WriteAsync(targetSlice, cts.Token).AsTask().GetAwaiter().GetResult();
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
            throw new TimeoutException($"Write timed out after {WriteTimeout} ms.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }

    public void DiscardInBuffer()
    {
        (byteDuplex as IReadBufferDiscardable)?.DiscardInputBuffer();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}