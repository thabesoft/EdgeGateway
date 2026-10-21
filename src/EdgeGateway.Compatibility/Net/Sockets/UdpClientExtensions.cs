#if NETSTANDARD2_0

using System.Runtime.InteropServices;

namespace System.Net.Sockets;

public static class UdpClientExtensions
{
    extension(UdpClient client)
    {
        public async ValueTask<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken)
        {
            var receiveTask = client.ReceiveAsync();

            // 如果令牌可取消，就竞速；否则直接等
            if (cancellationToken.CanBeCanceled)
            {
                var cancelTask = Task.Delay(Timeout.Infinite, cancellationToken);
                var completed = await Task.WhenAny(receiveTask, cancelTask).ConfigureAwait(false);

                if (completed == cancelTask)
                {
                    // 取消触发，但不要 Close 客户端。
                    // 注意：ReceiveAsync 任务仍会在后台等待，直到下一个数据包到来。
                    throw new OperationCanceledException(cancellationToken);
                }
            }

            return await receiveTask.ConfigureAwait(false);
        }


        public async ValueTask<int> SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
        {
            // 在开始发送前响应取消。发送操作本身极快，一旦开始就让它完成。
            cancellationToken.ThrowIfCancellationRequested();

            // 调用底层无令牌的 SendAsync（netstandard2.0 上只有 byte[] 重载）
            // 如果 data 背后是数组，直接复用；否则先 ToArray（Send 通常数据不大，可接受）
            if (MemoryMarshal.TryGetArray(data, out var segment)
                && segment.Array != null)
            {
                return await client.SendAsync(segment.Array, segment.Count)
                    .ConfigureAwait(false);
            }

            return await client.SendAsync(data.ToArray(), data.Length).ConfigureAwait(false);
        }
    }
}

#endif