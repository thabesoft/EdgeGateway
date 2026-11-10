using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal partial class LifecycleViewModel(ILifecycle lifecycle, ISukiToastManager sukiToastManager) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRunning))]
    public partial LifecycleState State { get; set; }

    public bool IsRunning => State == LifecycleState.Running;


    [RelayCommand]
    private async Task StartAsync()
    {
        try
        {
            await lifecycle.StartAsync();
            State = lifecycle.State;
        }
        catch(Exception ex)
        {
            sukiToastManager.CreateToast()
               .OfType(Avalonia.Controls.Notifications.NotificationType.Error)
               .WithTitle("启动失败")
               .WithContent(ex.Message)
               .Queue();
        }
    }
    [RelayCommand]
    private async Task StopAsync()
    {
        try
        {
            await lifecycle.StopAsync();
            State = lifecycle.State;
        }
        catch (Exception ex)
        {
            sukiToastManager.CreateToast()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Error)
                .WithTitle("停止失败")
                .WithContent(ex.Message)
                .Queue();
        }
    }
}