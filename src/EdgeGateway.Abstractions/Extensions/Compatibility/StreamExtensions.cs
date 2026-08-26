#if NETSTANDARD2_0

using System.Runtime.InteropServices;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System.IO;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

internal static class SystemIOExtensions
{
    extension(Stream stream)
    {
        public async ValueTask ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            int totalRead = 0;
            int length = buffer.Length;

            while (totalRead < length)
            {
                int bytesRead = 0;
                var segment = buffer[totalRead..];

                // netstandard2.0 下：将 Memory<byte> 转为 ArraySegment<byte>
                // 然后调用传统 Stream.ReadAsync(byte[], int, int)
                if (!MemoryMarshal.TryGetArray(segment, out ArraySegment<byte> arraySegment))
                {
                    byte[] temp = new byte[segment.Length];
                    int read = await stream.ReadAsync(temp, 0, temp.Length, cancellationToken)
                                            .ConfigureAwait(false);
                    if (read > 0)
                    {
                        temp.AsSpan(0, read).CopyTo(segment.Span);
                    }
                    bytesRead = read;
                }
                else
                {
                    bytesRead = await stream.ReadAsync(arraySegment.Array!, arraySegment.Offset, arraySegment.Count, cancellationToken)
                                            .ConfigureAwait(false);
                }

                if (bytesRead == 0)
                {
                    throw new EndOfStreamException("Unable to read exactly the requested number of bytes.");
                }

                totalRead += bytesRead;
            }
        }

        public async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> arraySegment))
            {
                await stream.WriteAsync(arraySegment.Array!, arraySegment.Offset, arraySegment.Count, cancellationToken)
                            .ConfigureAwait(false);
            }
            else
            {
                byte[] temp = buffer.ToArray();
                await stream.WriteAsync(temp, 0, temp.Length, cancellationToken)
                            .ConfigureAwait(false);
            }
        }
    }
}

#endif