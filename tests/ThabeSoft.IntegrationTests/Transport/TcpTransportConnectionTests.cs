using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;
using ThabeSoft.Lifecycles.DependencyInjection;
using ThabeSoft.Transports;
using ThabeSoft.Transports.DependencyInjection;
using ThabeSoft.Transports.Ethernet;
using ThabeSoft.Transports.Ethernet.DependencyInjection;

namespace ThabeSoft.IntegrationTests.Transport;


[TestClass]
public class TcpTransportConnectionTests
{
    [TestMethod]
    public async Task StartStop_RestartIsSupported()
    {
        var app = Host.CreateDefaultBuilder()
            .ConfigureServices(x => x
                .AddLogging()
                .AddLifecycles()
                .AddEthernetTransport()
                .AddTransports())
            .Build();

        var factory = app.Services.GetRequiredService<ITransportConnectionFactory>();

        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();

        var descriptor = Ethernet.Tcp((IPEndPoint)listener.LocalEndpoint);
        var connection = factory.Create(descriptor);

        try
        {
            await connection.StartAsync(TestContext.CancellationToken);
            await connection.StartAsync(TestContext.CancellationToken);   // 重复启动应幂等

            await connection.StopAsync(TestContext.CancellationToken);

            await connection.StartAsync(TestContext.CancellationToken);   // 停止后可再次启动

            // 服务端关闭后读取应抛异常（否则说明连接状态未感知）
            listener.Dispose();

            await Assert.ThrowsAsync<Exception>(async () =>
                await connection.ReadAsync(new byte[10], TestContext.CancellationToken));
        }
        finally
        {
            await connection.DisposeAsync();
        }
    }

    public TestContext TestContext { get; set; }
}
