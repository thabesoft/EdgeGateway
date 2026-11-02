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
        return null!;
    }
}