using CommunityToolkit.Mvvm.ComponentModel;

namespace ThabeSoft.EdgeGateway.App.ViewModels;

internal sealed partial class AddIpPortForm : ViewModelBase
{
    public IpConnectionType[] ConnectionTypeOptions { get; } = [IpConnectionType.Tcp, IpConnectionType.Udp];


    [ObservableProperty]
    public partial string Address { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int Port { get; set; } = 0;

    [ObservableProperty]
    public partial IpConnectionType ConnectionType { get; set; } = IpConnectionType.Tcp;
}