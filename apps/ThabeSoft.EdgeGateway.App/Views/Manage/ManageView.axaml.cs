using Avalonia.Controls;
using System.Net;
using ThabeSoft.EdgeGateway.App.Mocks;
using ThabeSoft.EdgeGateway.App.ViewModels;
using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.App.Views;

public partial class ManageView : UserControl
{
    public ManageView()
    {
        InitializeComponent();

        if (!Design.IsDesignMode) return;
        DataContext = CreateDesignData();
    }


    private static ManageViewModel CreateDesignData()
    {
        var gatewayViewModel = new GatewayViewModel(new DesignGateway(), port => new(port, device => new(device)));
        var vm = new ManageViewModel(NullDialogService.Instance, gatewayViewModel);

        var serialChannel = SerialChannelDescriptor.Create("COM2");
        vm.Gateway.AddPortCommand.Execute(serialChannel);

        var serialPort = vm.Gateway.Ports.FirstOrDefault(x => x.ChannelAddress == serialChannel.Address);
        serialPort?.AddDeviceFromProtocolCommand.Execute(ModbusProtocolDescriptor.Ascii);

        var ipChannel = IpChannelDescriptor.Tcp(IPEndPoint.Parse("127.0.0.1:8086"));
        vm.Gateway.AddPortCommand.Execute(ipChannel);

        var ipPort = vm.Gateway.Ports.FirstOrDefault(x => x.ChannelAddress == ipChannel.Address);
        ipPort?.AddDeviceFromProtocolCommand.Execute(ModbusProtocolDescriptor.Ip);

        return vm;
    }
}