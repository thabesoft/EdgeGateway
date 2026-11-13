using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal partial class LifecycleViewModel(
        ILifecycleWithState lifecycle,
        ISukiToastManager sukiToastManager
    ) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRunning))]
    public partial LifecycleState State { get; set; } = lifecycle.State;

    public bool IsRunning => State == LifecycleState.Running;


    [RelayCommand]
    private async Task StartAsync()
    {
        await sukiToastManager
            .ExecuteWithToastAsync(StartAsync, "启动失败", TimeSpan.FromSeconds(60));
    }
    [RelayCommand]
    private async Task StopAsync()
    {
        await sukiToastManager
            .ExecuteWithToastAsync(StopAsync, "停止失败", TimeSpan.FromSeconds(60));
    }


    public virtual async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await lifecycle.StartAsync(cancellationToken);
        }
        finally
        {
            OnUpdateState();
        }
    }
    public virtual async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await lifecycle.StopAsync(cancellationToken);
        }
        finally
        {
            OnUpdateState();
        }
    }

    public void OnUpdateState() => State = lifecycle.State;
}