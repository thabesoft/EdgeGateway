using Avalonia.Controls;
using System.Net;
using ThabeSoft.EdgeGateway.App.ViewModels;
using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.App.Views;

public partial class PortView : UserControl
{
    public PortView()
    {
        InitializeComponent();

        if (!Design.IsDesignMode) return;


        DataContext = new PortViewModel()
        {
            Ports =
            [
                new()
                {
                    ChannelAddress = ChannelAddress.Serial("COM3"),
                    IsOpen = true,
                    Devices = [new() { DisplayName = "Modbus-Rtu", IsEnabled = true, ProtocolType = ProtocolType.Modbus }]
                },
                new()
                {
                     DisplayName = "设备名称",
                    ChannelAddress = ChannelAddress.Ethernet(new(IPAddress.Loopback, 1234)),
                    IsOpen = true,
                    Devices = [new() { DisplayName = "Modbus-Ip", IsEnabled = true, ProtocolType = ProtocolType.Modbus }]
                }
            ]
        };
    }
}