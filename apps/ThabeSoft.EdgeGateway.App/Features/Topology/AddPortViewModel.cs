using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.App.Features.Gateway;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;


internal sealed partial class AddPortViewModel(ISukiDialog dialog, ISukiToastManager sukiToastManager, GatewayViewModel gatewayViewModel) : ObservableObject
{
    public IpPortFormViewModel IpForm { get; } = new();
    public SerialPortFormViewModel SerialForm { get; } = new();


    [ObservableProperty]
    public partial bool IsSelectedIpFrom { get; set; } = true;

    [ObservableProperty]
    public partial bool IsSelectedSerialFrom { get; set; }


    [RelayCommand]
    private void Create()
    {
        try
        {
            ITransportDescriptor? descriptor = null;

            if (IsSelectedIpFrom) descriptor = IpForm.Build();
            else if (IsSelectedSerialFrom) descriptor = SerialForm.Build();

            if (descriptor == null) return;
            gatewayViewModel.AddPortCommand.Execute(descriptor);

            dialog.Dismiss();
        }
        catch(Exception ex)
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