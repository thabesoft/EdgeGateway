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
            .ExecuteWithToastAsync(async ct =>
            {
                await OnStartingAsync(ct);

                await lifecycle.StartAsync(ct);
                State = lifecycle.State;

                await OnStartedAsync(ct);

            }, "启动失败");
    }
    [RelayCommand]
    private async Task StopAsync()
    {
        await sukiToastManager
            .ExecuteWithToastAsync(async ct =>
            {
                await OnStartingAsync(ct);

                await lifecycle.StopAsync(ct);
                State = lifecycle.State;

                await OnStopedAsync(ct);
            }, "停止失败");
    }


    protected virtual ValueTask OnStartingAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
    protected virtual ValueTask OnStartedAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
    protected virtual ValueTask OnStopingAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
    protected virtual ValueTask OnStopedAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
}