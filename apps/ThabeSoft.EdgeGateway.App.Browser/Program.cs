using Avalonia;
using Avalonia.Browser;
using Microsoft.Extensions.Hosting;

namespace ThabeSoft.EdgeGateway.App.Browser;

internal sealed class Program
{
    private static Task Main(string[] _) => BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure(() => new App(Host.CreateDefaultBuilder().Build()));
}