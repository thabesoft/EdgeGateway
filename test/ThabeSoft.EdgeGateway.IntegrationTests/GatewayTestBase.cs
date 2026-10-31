using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.EdgeGateway.IntegrationTests;

public class GatewayTestBase
{
    public TestContext TestContext { get; set; } = null!;
    protected IGateway Gateway { get; set; } = null!;
    protected IServiceProvider Services { get; set; } = null!;



    [TestInitialize]
    public async Task InitializeAsync()
    {
        var descriptors = new ServiceCollection();
        descriptors.AddLogging();
        descriptors.AddEdgeGateway();
        OnConfigureServices(descriptors);

        Services = descriptors.BuildServiceProvider();
        Gateway = Services.GetRequiredService<IGateway>();

        await Gateway.StartAsync(TestContext.CancellationToken);
    }

    [TestCleanup]
    public async Task CleanupAsync()
    {
        if (Services is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (Services is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    protected virtual void OnConfigureServices(ServiceCollection descriptors) { }
}