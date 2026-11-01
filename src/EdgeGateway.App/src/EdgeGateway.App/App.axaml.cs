using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EdgeGateway.App.DataTemplates;
using EdgeGateway.App.ViewModels;
using EdgeGateway.App.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway;

namespace EdgeGateway.App;

public class App : Application
{
    private readonly IHost _host;


    public App(Action<IHostBuilder> buildAction)
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureLogging((_, builder) => builder.SetMinimumLevel(LogLevel.Trace))
            .ConfigureServices(x => x
                .AddEdgeGateway()
                .AddSerial()
                .AddEthernet()
                .AddModbus()
                .AddEdgeGatewayHost()
                .AddEdgeGatewayApp());

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
        if (!Design.IsDesignMode)
            _host.Start();

        DataTemplates.Add(Services.GetRequiredService<ViewModelTmpleate>());

        InitializeMainView();
        base.OnFrameworkInitializationCompleted();
    }

    private void InitializeMainView()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindowViewModel = Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow { DataContext = mainWindowViewModel };
            desktop.Exit += delegate { _host.StopAsync().GetAwaiter().GetResult(); };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var mainViewModel = Services.GetRequiredService<MainViewModel>();
            singleViewPlatform.MainView = new MainView { DataContext = mainViewModel };
        }
    }
}
