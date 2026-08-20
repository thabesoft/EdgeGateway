using FluentResults;
using Microsoft.Extensions.Logging;
using System.IO.Pipelines;
using System.Net.Sockets;

namespace EdgeGateway.Ports.Ethernet;


//internal class UdpPortAdapter(EthernetPortConfig config, ILogger<UdpPortAdapter> logger) : Port(config.Address)
//{
//    private readonly Pipe _pipe = new();
//    private CancellationTokenSource _receiveCts = new();
//    private readonly UdpClient _udpClient = config.Local is null ? new() : new(config.Local);


//    protected override ValueTask<Result> OpenAsync(CancellationToken cancellationToken = default)
//    {
//        _udpClient.Connect(config.Remote.Address, config.Remote.Port);
//        Task.Run(() => ReceiveLoopAsync(_receiveCts.Token), _receiveCts.Token);

//        return ValueTask.FromResult(Result.Ok());
//    }
//    protected override async ValueTask<Result> CloseAsync(CancellationToken cancellationToken = default)
//    {
//#if NETSTANDARD2_0
//        _receiveCts.Cancel();
//#else
//        await _receiveCts.CancelAsync();
//#endif
//        _udpClient.Close();

//        return Result.Ok();
//    }
//    public override async ValueTask DisposeAsync()
//    {
//        try
//        {
//            var closeResult = await CloseAsync(new CancellationTokenSource(5000).Token);
//            if (closeResult.IsFailed) logger.LogError("关闭失败: {msg}", closeResult);

//            _udpClient.Dispose();
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "释放失败");
//        }
//    }

//    public override async ValueTask<Result> ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
//    {

//    }

//    public override ValueTask<Result> WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
//    {
//        throw new NotImplementedException();
//    }


//    private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
//    {
//        while (!cancellationToken.IsCancellationRequested)
//        {
//            try
//            {
//                // 1. 接收 UDP 数据报
//                var result = await _udpClient.ReceiveAsync();
//                var data = result.Buffer;

//                // 2. 获取 PipeWriter
//                var writer = _pipe.Writer;

//                // 3. 写入数据到 Pipe
//                var memory = writer.GetMemory(data.Length);
//                data.CopyTo(memory);
//                writer.Advance(data.Length);

//                // 4. 提交数据
//                await writer.FlushAsync(cancellationToken);
//            }
//            catch (OperationCanceledException)
//            {
//                break;
//            }
//            catch (Exception ex)
//            {
//                // 记录错误，继续接收
//                logger.LogError(ex, "UDP 接收错误: {msg}", ex.Message);
//            }
//        }

//        // 完成写入
//        await _pipe.Writer.CompleteAsync();
//    }
//}
