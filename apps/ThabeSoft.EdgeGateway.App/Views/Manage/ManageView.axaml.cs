using Avalonia.Controls;
using System.Net;
using ThabeSoft.EdgeGateway.App.Mocks;
using ThabeSoft.EdgeGateway.App.ViewModels;
using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
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


        // Serial
        var serialPortVm = AddPort(gatewayViewModel, PortDescriptor.Create(SerialChannelDescriptor.Create("COM2")));
        var serialPortVm_MosbusRtuDevice = AddDevice(serialPortVm, DeviceDescriptor.Create(ModbusProtocolDescriptor.Rtu));
        var serialPortVm_MosbusAsciiDevice = AddDevice(serialPortVm, DeviceDescriptor.Create(ModbusProtocolDescriptor.Ascii));

        // Ip-Tcp
        var ipTcpPortVm = AddPort(gatewayViewModel, PortDescriptor.Create(IpChannelDescriptor.Tcp(new IPEndPoint(IPAddress.Loopback, 8080))));
        var ipTcpPortVm_MosbusIpDevice = AddDevice(ipTcpPortVm, DeviceDescriptor.Create(ModbusProtocolDescriptor.Ip));

        // Ip-Udp
        var ipUdpPortVm = AddPort(gatewayViewModel, PortDescriptor.Create(IpChannelDescriptor.Udp(new IPEndPoint(IPAddress.Loopback, 8080))));
        var ipUdpPortVm_MosbusIpDevice = AddDevice(ipUdpPortVm, DeviceDescriptor.Create(ModbusProtocolDescriptor.Ip));

        return vm;



        static PortViewModel? AddPort(GatewayViewModel? gatewayViewModel, IPortDescriptor portDescriptor)
        {
            gatewayViewModel?.AddPortCommand.Execute(portDescriptor);

            return gatewayViewModel?.Ports
                .FirstOrDefault(x => x.Id == portDescriptor.PortId);
        }

        static DeviceViewModel? AddDevice(PortViewModel? portViewModel, IDeviceDescriptor deviceDescriptor)
        {
            portViewModel?.AddDeviceCommand.Execute(deviceDescriptor);

            return portViewModel?.Devices
                .FirstOrDefault(x => x.Id == deviceDescriptor.DeviceId);
        }
    }
}