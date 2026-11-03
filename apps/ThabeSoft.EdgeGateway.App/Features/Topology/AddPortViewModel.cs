using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Dialogs;
using ThabeSoft.EdgeGateway.App.ViewModels;
using ThabeSoft.EdgeGateway.Channels;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;


internal sealed partial class AddPortViewModel(ISukiDialog dialog) : ObservableObject
{
    public IpPortFormViewModel IpForm { get; } = new();
    public SerialPortFormViewModel SerialForm { get; } = new();


    [ObservableProperty]
    public partial bool IsSelectedIpFrom { get; set; } = true;

    [ObservableProperty]
    public partial bool IsSelectedSerialFrom { get; set; }


    public IChannelDescriptor Build()
    {
        if (IsSelectedIpFrom) return IpForm.Build();
        if (IsSelectedSerialFrom) return SerialForm.Build();

        throw new InvalidOperationException("无法构建");
    }


    [RelayCommand]
    private void CloseDialog()
    {
        dialog.Dismiss();
    }
}