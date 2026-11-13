using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ThabeSoft.EdgeGateway.App.Designs;


internal static class DesignHost
{
    private readonly static IServiceProvider? _services;

#if DEBUG
    private readonly static IHost? _host;

    static DesignHost()
    {
        if (!Design.IsDesignMode) return;

        var builder = Host.CreateDefaultBuilder()
            .ConfigureLogging((_, builder) =>
                builder.ClearProviders().AddDebug().SetMinimumLevel(LogLevel.Trace))
            .UseEdgeGatewayApp();

        _host = builder.Build();
        _services = _host.Services;
        _host.Start();
    }

#endif


    public static T? GetService<T>()
        where T : notnull
    {
        if (_services is null) return default;
        if (!Design.IsDesignMode) return default;

        return _services.GetService<T>();
    }

    public static bool SetDataContext<T>(IDataContextProvider dataContextProvider)
         where T : notnull
    {
        var dataContext = GetService<T>();
        if (dataContext is null) return false;

        dataContextProvider.DataContext = dataContext;
        return true;
    }
}