using EdgeGateway.Options;
using FluentResults;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace EdgeGateway.Ports.Ethernet;

internal class TcpPortAdapter(EthernetPortOptions config, ILogger<TcpPortAdapter> logger) : StreamPort(config.Address)
{
    private readonly TcpClient _tcpClient = config.Local is null ? new() : new(config.Local);


    protected override Stream Stream => _tcpClient.GetStream();

    protected override ValueTask<Result> OpenAsync(CancellationToken cancellationToken = default)
    {
        var resultTask = Result.Try(() => _tcpClient.ConnectAsync(config.Remote.Address, config.Remote.Port));
        return new ValueTask<Result>(resultTask);
    }
    protected override ValueTask<Result> CloseAsync(CancellationToken cancellationToken = default)
    {
        var result = Result.Try(_tcpClient.Close);
        return new ValueTask<Result>(result);
    }
    public override async ValueTask DisposeAsync()
    {
        try
        {
            var closeResult = await CloseAsync(new CancellationTokenSource(5000).Token);
            if (closeResult.IsFailed) logger.LogError("关闭失败: {msg}", closeResult);

            _tcpClient.Dispose();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "释放失败");
        }
    }
}
