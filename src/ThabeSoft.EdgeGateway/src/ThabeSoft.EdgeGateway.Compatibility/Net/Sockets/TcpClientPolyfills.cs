#if NETSTANDARD2_0

namespace System.Net.Sockets;

public static class TcpClientExtensions
{
    extension(TcpClient client)
    {
        public async ValueTask ConnectAsync(IPAddress address, int port, CancellationToken cancellationToken)
        {
            using var _ = cancellationToken.Register(client.Close);

            try
            {
                await client.ConnectAsync(address, port);
            }
            catch (Exception ex) when (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException("连接已取消。", ex);
            }
        }
    }
}

#endif