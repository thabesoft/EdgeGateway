using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.Transports;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;


internal sealed partial class AddPortViewModel(
        ISukiDialog dialog,
        ISukiToastManager sukiToastManager,
        IGateway gateway,
        ISerialPortNamesProvider serialPortNamesProvider
    ) : ObservableObject
{
    public IpPortViewModel IpForm { get; } = new();
    public SerialPortViewModel SerialForm { get; } = new(serialPortNamesProvider);


    [ObservableProperty]
    public partial bool IsSelectedIpFrom { get; set; } = true;

    [ObservableProperty]
    public partial bool IsSelectedSerialFrom { get; set; }


    [RelayCommand]
    private async Task Create()
    {
        try
        {
            TransportDescriptor? descriptor = null;

            if (IsSelectedIpFrom) descriptor = IpForm.Build();
            else if (IsSelectedSerialFrom) descriptor = SerialForm.Build();

            if (descriptor == null) return;
            await gateway.AddPortAsync(descriptor);

            dialog.Dismiss();
        }
        catch (Exception ex)
        {
            if (IsSelectedIpFrom) IpForm.ValidateAllProperty();
            else if (IsSelectedSerialFrom) SerialForm.ValidateAllProperty();

            sukiToastManager.CreateToast()
                .OfType(Avalonia.Controls.Notifications.NotificationType.Error)
                .WithTitle("创建失败")
                .WithContent(ex.Message)
                .Dismiss().After(TimeSpan.FromSeconds(3))
                .Queue();
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        dialog.Dismiss();
    }
}