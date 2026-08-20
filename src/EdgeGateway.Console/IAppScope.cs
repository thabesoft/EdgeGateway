
using Microsoft.Extensions.DependencyInjection;

namespace EdgeGateway.Console;


public interface IAppScope
{
    IServiceProvider ServiceProvider { get; }
}

public sealed class AppScope : IAppScope, IDisposable, IAsyncDisposable
{
    private readonly AsyncServiceScope _serviceScope;

    public IServiceProvider ServiceProvider { get; private set;  }

    public AppScope(IServiceScopeFactory scopeFactory)
    {
        _serviceScope = scopeFactory.CreateAsyncScope();
        ServiceProvider = _serviceScope.ServiceProvider;
    }


    public void Dispose()  => _serviceScope.Dispose();
    public ValueTask DisposeAsync() => _serviceScope.DisposeAsync();
}