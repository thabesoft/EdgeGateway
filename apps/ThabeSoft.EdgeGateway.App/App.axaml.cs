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


public class App : Application, IAsyncDisposable
{
    private readonly IHost _host;
    private readonly ILogger<App> _logger;
    private Task? _luanchHostTask;
    private readonly CancellationTokenSource _luanchHostTaskCts = new();


    public App(IHost host)
    {
        _host = host;
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

        // 初始化数据模板
        var viewLocator = Services.GetRequiredService<Common.ViewLocator>();
        DataTemplates.Add(viewLocator);

        // 主窗口
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var viewFactory = Services.GetRequiredService<ViewFactory>();
            desktop.MainWindow = viewFactory.CreateView<MainWindowViewModel>() as Window;
        }

        // 启动 Host
        _luanchHostTask = Task.Run(async () =>
        {
            try
            {
                await _host.StartAsync(_luanchHostTaskCts.Token);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Host启动失败");
            }

        }, _luanchHostTaskCts.Token);
    }


    public async ValueTask DisposeAsync()
    {
        await _luanchHostTaskCts.CancelAsync();

        try
        {
            if (_luanchHostTask is not null) await _luanchHostTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Host启动任务释放失败");
        }

        GC.SuppressFinalize(this);
    }
}
