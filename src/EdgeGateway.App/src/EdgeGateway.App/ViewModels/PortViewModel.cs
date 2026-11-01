using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using EdgeGateway.App.Models;
using EdgeGateway.App.Servies;
using System.IO.Ports;
using System.Net;
using ThabeSoft.EdgeGateway;
using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Ports;

namespace EdgeGateway.App.ViewModels;

public partial class PortViewModel(IDialogService dialogService, IGateway gateway) : ViewModelBase
{
    public AvaloniaList<PortItemViewModel> Ports { get; } = [];

    [RelayCommand]
    private void AddPort()
    {
        var content = new AddPortViewModel();
        DialogContent dialogContent = null!;
        dialogContent = DialogContent.OkCancel(content: content, okCallback: OnCreate, cancelCallback: OnCancel, "添加端口");

        dialogService.Show(dialogContent);


        void OnCreate()
        {
            if (content.IsSelectedIpFrom)
            {
                var from = content.IpForm;
                var iPEndPoint = new IPEndPoint(IPAddress.Parse(from.Address), from.Port);
                var ipChannelDescriptor = IpChannelDescriptor.Create(content.IpForm.ConnectionType, iPEndPoint);

                var port = gateway.AddPort(ipChannelDescriptor);
                Ports.Add(new PortItemViewModel(port));
            }
            else if (content.IsSelectedSerialFrom)
            {
                var from = content.SerialForm;
                var serialChannelDescriptor = SerialChannelDescriptor.Create(from.PortName, from.BaudRate, from.Parity, from.DataBits, from.StopBits);
                var port = gateway.AddPort(serialChannelDescriptor);
                Ports.Add(new PortItemViewModel(port));
            }

            dialogService.Close(dialogContent);
        }
        void OnCancel()
        {
            dialogService.Close(dialogContent);
        }
    }


}

public sealed class PortItemViewModel(IPort port)
{
    public PortId Id => port.Id;
    public ConnectionType ConnectionType => port.ConnectionType;
    public bool IsOpen => port.IsOpen;
    public int DeviceCount => port.Devices.Count;
}