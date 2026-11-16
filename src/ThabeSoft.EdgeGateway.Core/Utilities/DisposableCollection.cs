using Microsoft.Extensions.Logging;

namespace ThabeSoft.EdgeGateway.Utilities;


internal readonly struct DisposableCollection(object[] objects, ILogger? logger = null) : IAsyncDisposable
{
    public static DisposableCollection From(params object[] objects) =>
        new(objects);
    public static DisposableCollection From(ILogger logger, params object[] objects) =>
        new(objects, logger);


    public async ValueTask DisposeAsync()
    {
        foreach (var i in objects)
        {
            if (i is IAsyncDisposable asyncDisposable)
                await asyncDisposable.TryDisposeAsync(logger);
            else if (i is IDisposable disposable)
                disposable.TryDispose(logger);
        }
    }
}