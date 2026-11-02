using CommunityToolkit.Mvvm.ComponentModel;
using System.IO.Ports;

namespace ThabeSoft.EdgeGateway.App.ViewModels;

internal sealed partial class SerialPortFormViewModel : ViewModelBase
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
