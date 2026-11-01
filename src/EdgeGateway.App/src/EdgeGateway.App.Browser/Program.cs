using Avalonia;
using Avalonia.Browser;

namespace EdgeGateway.App.Browser;

internal sealed class Program
{
    private static Task Main(string[] _) => BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}