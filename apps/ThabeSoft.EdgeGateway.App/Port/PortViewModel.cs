using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Net;
using ThabeSoft.EdgeGateway.App.Dialog;
using ThabeSoft.EdgeGateway.App.Navigation;
using ThabeSoft.EdgeGateway.App.Port.Device;
using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.App.Port;

public partial class PortViewModel : ViewModelBase, IRecipient<PortRemovedMessage>
{
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly IGateway _gateway;

    public PortViewModel(IDialogService dialogService, INavigationService navigationService, IGateway gateway)
    {
        _gateway = gateway;
        _dialogService = dialogService;
        _navigationService = navigationService;

        WeakReferenceMessenger.Default.Register(this);
    }

    public AvaloniaList<PortItemViewModel> Ports { get; } = [];


    [RelayCommand]
    private void AddPort()
    {
        var content = new AddPortViewModel();
        DialogContent dialogContent = null!;
        dialogContent = DialogContent.OkCancel(content: content, okCallback: OnCreate, cancelCallback: OnCancel, "添加端口");

        _dialogService.Show(dialogContent);


        void OnCreate()
        {
            if (content.IsSelectedIpFrom)
            {
                var from = content.IpForm;
                var iPEndPoint = new IPEndPoint(IPAddress.Parse(from.Address), from.Port);
                var ipChannelDescriptor = IpChannelDescriptor.Create(content.IpForm.ConnectionType, iPEndPoint);

                var port = _gateway.AddPort(ipChannelDescriptor);
                AddPortViewModel(port);
            }
            else if (content.IsSelectedSerialFrom)
            {
                var from = content.SerialForm;
                var serialChannelDescriptor = SerialChannelDescriptor.Create(from.PortName, from.BaudRate, from.Parity, from.DataBits, from.StopBits);

                var port = _gateway.AddPort(serialChannelDescriptor);
                AddPortViewModel(port);
            }

            _dialogService.Close(dialogContent);
        }
        void OnCancel()
        {
            _dialogService.Close(dialogContent);
        }
    }


    private void AddPortViewModel(IPort port)
    {
        Ports.Add(new PortItemViewModel(port, _gateway, _navigationService));
    }
    void IRecipient<PortRemovedMessage>.Receive(PortRemovedMessage message)
    {
        var portVm = Ports.FirstOrDefault(x => x.Id == message.PortId);
        if (portVm is null) return;

        Ports.Remove(portVm);
    }
}

public sealed partial class PortItemViewModel(IPort port, IGateway gateway, INavigationService navigationService) : ViewModelBase
{
    public PortId Id => port.Id;
    public ConnectionType ConnectionType => port.ConnectionType;
    public bool IsOpen => port.IsOpen;
    public int DeviceCount => port.Devices.Count;


    [RelayCommand]
    private async Task OpenAsync()
    {
        await port.OpenAsync();
        OnPropertyChanged(nameof(IsOpen));
    }

    [RelayCommand]
    private async Task CloseAsync()
    {
        await port.CloseAsync();
        OnPropertyChanged(nameof(IsOpen));
    }

    [RelayCommand]
    private async Task RemoveAsync()
    {
        await gateway.RemovePortAsync(Id);
        WeakReferenceMessenger.Default.Send(new PortRemovedMessage(Id));
    }

    [RelayCommand]
    private async Task OpenDeviceAsync()
    {
        var deviceViewModel = new DeviceViewModel(port);
        navigationService.Navigate(deviceViewModel);
    }
}