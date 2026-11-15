using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;
using ThabeSoft.EdgeGateway.DependencyInjection;

namespace ThabeSoft.EdgeGateway.Transports;


[TestClass]
public class TcpTransportConnectionTests
{
    [TestMethod]
    public async Task Fuck()
    {
        var app = Host.CreateDefaultBuilder()
            .ConfigureServices(x => x
                .AddLogging()
                .AddEdgeGatewayEthernet()
                .AddEdgeGatewayTransport()
            )
            .Build();
        var resolver = app.Services.GetRequiredService<ITransportConnectionFactoryResolver>();



        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();

        var transportDescriptor = Ethernet.Tcp((IPEndPoint)listener.LocalEndpoint);
        var transpor = resolver.Resolve(transportDescriptor);

        await transpor.StartAsync(TestContext.CancellationToken);
        await transpor.StartAsync(TestContext.CancellationToken);
        
        await transpor.StopAsync(TestContext.CancellationToken);
        await transpor.StartAsync(TestContext.CancellationToken);


        listener.Dispose();
        try
        {
            await transpor.ReadAsync(new byte[10], TestContext.CancellationToken);
        }
        catch
        {
            
        }

        await transpor.DisposeAsync();
    }

    public TestContext TestContext { get; set; }
}
