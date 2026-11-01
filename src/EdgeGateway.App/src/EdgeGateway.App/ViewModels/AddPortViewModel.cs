using CommunityToolkit.Mvvm.ComponentModel;
using System.IO.Ports;
using ThabeSoft.EdgeGateway;

namespace EdgeGateway.App.ViewModels;


internal sealed partial class AddPortViewModel : ViewModelBase
{
    public AddIpPortForm IpForm { get; } = new();
    public AddSerialPortForm SerialForm { get; } = new();


    [ObservableProperty]
    public partial bool IsSelectedIpFrom { get; set; } = true;

    [ObservableProperty]
    public partial bool IsSelectedSerialFrom { get; set; }
}


internal sealed partial class AddSerialPortForm : ViewModelBase
{
    public Parity[] ParitysOptions { get; } = [Parity.None, Parity.Odd, Parity.Even, Parity.Mark, Parity.Space];
    public StopBits[] StopBitsOptions { get; } = [StopBits.One, StopBits.OnePointFive, StopBits.Two];


    [ObservableProperty]
    public partial string PortName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial Parity Parity { get; set; } = Parity.None;

    [ObservableProperty]
    public partial int BaudRate { get; set; } = 9600;

    [ObservableProperty]
    public partial int DataBits { get; set; } = 8;

    [ObservableProperty]
    public partial StopBits StopBits { get; set; } = StopBits.One;
}

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