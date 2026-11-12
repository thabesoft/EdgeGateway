using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

    public override void OnFrameworkInitializationCompleted()
    {
        Dispatcher.UIThread.UnhandledException += (s, e) =>
        {
            _logger.LogError(e.Exception, "UI线程未处理异常");
            e.Handled = true;
        };
        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            _logger.LogError(e.Exception, "未观察到的Task异常");
            e.SetObserved();
        };



        var viewFactory = Services.GetRequiredService<ViewFactory>();
        var viewLocator = Services.GetRequiredService<Common.ViewLocator>();


        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DataTemplates.Add(viewLocator);
            desktop.MainWindow = viewFactory.CreateView<MainWindowViewModel>() as Window;
        }

        Task.Run(async () =>
        {
            try
            {
                await _host.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Host启动失败");

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                        desktop.Shutdown(-1);
                });
            }
        });
    }
}
