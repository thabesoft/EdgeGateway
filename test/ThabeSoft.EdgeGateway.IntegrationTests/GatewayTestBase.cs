using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.EdgeGateway;

public class GatewayTestBase
{
    public TestContext TestContext { get; set; } = null!;
    protected IGateway Gateway { get; set; } = null!;
    protected IServiceProvider Services { get; set; } = null!;



    [TestInitialize]
    public async ValueTask InitializeAsync()
    {
        var descriptors = new ServiceCollection();
        descriptors.AddLogging();
        descriptors.AddEdgeGateway();
        OnConfigureServices(descriptors);

        Services = descriptors.BuildServiceProvider();
        Gateway = Services.GetRequiredService<IGateway>();

        await Gateway.StartAsync(TestContext.CancellationToken);
        await OnInitializeAsync(TestContext.CancellationToken);
    }

    [TestCleanup]
    public async ValueTask CleanupAsync()
    {
        await OnCleanupAsync();

        if (Services is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (Services is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    protected virtual ValueTask OnInitializeAsync(CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;
    protected virtual ValueTask OnCleanupAsync() =>
        ValueTask.CompletedTask;

    protected virtual void OnConfigureServices(ServiceCollection descriptors) { }
}