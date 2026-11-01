using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using EdgeGateway.App.Servies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EdgeGateway.App.ViewModels;



public partial class MainViewModel : INavigationService
{
    [ObservableProperty]
    public partial NavigationMenuItem? SelectedMenu { get; set; }
    public AvaloniaList<NavigationMenuItem> Menus { get; } =
    [
        NavigationMenuItem.Create<DashboardViewModel>("主页"),
        NavigationMenuItem.Create<PortViewModel>("端口设置"),
    ];


    [ObservableProperty]
    public partial ViewModelBase? Content { get; set; }


    public void Navigate(Type type)
    {
        var content = services.GetRequiredService(type) as ViewModelBase;
        if(content is null)
        {
            logger.LogError("导航失败, 无法获取内容实例: {Type}", type.FullName);
            return;
        }

        Navigate(content);
    }
    public void Navigate(ViewModelBase vm)
    {
        if (Content?.Equals(vm) == true) return;
        Content = vm;
    }

    partial void OnSelectedMenuChanged(NavigationMenuItem? value)
    {
        if (value is null || value.Target is null) return;
        Navigate(value.Target);
    }
}

public sealed class NavigationMenuItem
{
    public required string Title { get; init; }
    public required Type Target { get; init; }


    private NavigationMenuItem() { }
    public static NavigationMenuItem Create<T>(string title) where T : ViewModelBase
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return new NavigationMenuItem
        {
            Title = title,
            Target = typeof(T)
        };
    }
}