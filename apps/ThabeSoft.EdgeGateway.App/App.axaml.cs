using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SukiUI.Controls;
using SukiUI.Dialogs;
using SukiUI.Enums;
using ThabeSoft.EdgeGateway.App.Common;

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
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var viewFactory = Services.GetRequiredService<ViewFactory>();
            var viewLocator = Services.GetRequiredService<ViewLocator>();

            DataTemplates.Add(viewLocator);
            desktop.MainWindow = viewFactory.CreateView<MainWindowViewModel>() as Window;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            var p = new Panel();
            p.Children.Add(new SukiBackground() { Style = SukiBackgroundStyle.Bubble });
            var dialogManager = new SukiDialogManager();

            singleView.MainView = new SukiMainHost()
            {
                Hosts = [
                    new SukiDialogHost
                    {
                        Manager = dialogManager
                    }
                ],
                Content = p
            };
        }

        _host.Start();
        base.OnFrameworkInitializationCompleted();
    }
}
