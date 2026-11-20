using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;
using ThabeSoft.EdgeGateway.DependencyInjection;
using ThabeSoft.Transports.Ethernet;
using ThabeSoft.Transports.Ethernet.DependencyInjection;
using ThabeSoft.EdgeGateway.Extensions;

namespace ThabeSoft.Transports;


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
        var factory = app.Services.GetRequiredService<ITransportConnectionFactory>();



        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();

        var transportDescriptor = EthernetBuilder.Tcp((IPEndPoint)listener.LocalEndpoint);
        var transpor = factory.Create(transportDescriptor);

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