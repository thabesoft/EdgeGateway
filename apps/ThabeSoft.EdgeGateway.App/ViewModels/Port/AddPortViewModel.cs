using CommunityToolkit.Mvvm.ComponentModel;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


internal sealed partial class AddPortViewModel : ViewModelBase
{
    public AddIpPortForm IpForm { get; } = new();
    public AddSerialPortForm SerialForm { get; } = new();


    [ObservableProperty]
    public partial bool IsSelectedIpFrom { get; set; } = true;

    [ObservableProperty]
    public partial bool IsSelectedSerialFrom { get; set; }
}
