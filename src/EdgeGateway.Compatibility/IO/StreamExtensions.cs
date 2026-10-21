#if NETSTANDARD2_0

using System.Buffers;
using System.Runtime.InteropServices;

namespace System.IO;

public static class SystemIOExtensions
{
    extension(Stream stream)
    {
        public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(buffer);

            // 如果 Memory<byte> 背后是数组，直接复用，零拷贝
            if (MemoryMarshal.TryGetArray<byte>(buffer, out var segment)
                && segment.Array != null)
            {
                return await stream.ReadAsync(segment.Array, segment.Offset, segment.Count, cancellationToken)
                    .ConfigureAwait(false);
            }

            // 否则租借临时数组
            byte[] rented = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                int read = await stream.ReadAsync(rented, 0, buffer.Length, cancellationToken)
                    .ConfigureAwait(false);
                rented.AsSpan(0, read).CopyTo(buffer.Span);
                return read;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }

        [Obsolete]
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