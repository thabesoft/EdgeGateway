using CommunityToolkit.Mvvm.ComponentModel;
using ThabeSoft.EdgeGateway.Channels;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


internal sealed partial class AddPortViewModel : ViewModelBase
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
}