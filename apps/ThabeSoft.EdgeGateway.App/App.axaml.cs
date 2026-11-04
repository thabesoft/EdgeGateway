using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SukiUI.Controls;
using SukiUI.Dialogs;
using SukiUI.Enums;
using ThabeSoft.EdgeGateway.App.Common;

namespace ThabeSoft.EdgeGateway.App;

public class App : Application
{
    private readonly IHost _host;
    private readonly ILogger<App> _logger;


    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .UseEdgeGatewayApp()
            .Build();

        _logger = _host.Services.GetRequiredService<ILogger<App>>();
    }


    public IServiceProvider Services => _host.Services;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        try
        {
            await _host.StartAsync();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var viewFactory = Services.GetRequiredService<ViewFactory>();
                var viewLocator = Services.GetRequiredService<ViewLocator>();

                DataTemplates.Add(viewLocator);

                Dispatcher.Invoke(() =>
                    desktop.MainWindow = viewFactory.CreateView<MainWindowViewModel>() as Window);
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "启动失败");
        }
    }
}
