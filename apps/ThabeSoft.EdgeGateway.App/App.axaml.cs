using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.App.DataTmpleates;
using ThabeSoft.EdgeGateway.App.ViewModels;
using ThabeSoft.EdgeGateway.App.Views;

namespace ThabeSoft.EdgeGateway.App;

public class App : Application
{
    private readonly IHost _host;


    public App(Action<IHostBuilder> buildAction)
    {
        var builder = Host.CreateDefaultBuilder()
            .UseEdgeGatewayApp();

        buildAction?.Invoke(builder);
        _host = builder.Build();
    }
    public App() : this(delegate { }) { }


    public IServiceProvider Services => _host.Services;


    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        DataTemplates.Add(Services.GetRequiredService<ViewModelDataTmpleate>());
        InitializeMainView();

        _host.Start();
    }

    private void InitializeMainView()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindowViewModel = Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow { DataContext = mainWindowViewModel };
            desktop.Exit += async delegate
            {
                using var cts = new CancellationTokenSource(3000);
                try
                {
                    await _host.StopAsync(cts.Token);
                }
                catch (Exception ex)
                {
                    var logger = Services.GetRequiredService<ILogger<App>>();
                    logger.LogError(ex, "关闭失败");
                }
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var mainViewModel = Services.GetRequiredService<MainViewModel>();
            singleViewPlatform.MainView = new MainView { DataContext = mainViewModel };
        }
    }
}
