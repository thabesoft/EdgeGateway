using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Net;
using ThabeSoft.EdgeGateway.App.Messages;
using ThabeSoft.EdgeGateway.App.Mocks;
using ThabeSoft.EdgeGateway.App.Models;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


public partial class PortViewModel : ViewModelBase, IRecipient<PortRemovedMessage>
{
    private readonly IDialogService _dialogService;
    private readonly PortSummaryViewModelFactory _portSummaryViewModelFactory;
    private readonly IGateway _gateway;


    public PortViewModel(IDialogService dialogService, IGateway gateway, PortSummaryViewModelFactory portSummaryViewModelFactory)
    {
        _gateway = gateway;
        _dialogService = dialogService;
        _portSummaryViewModelFactory = portSummaryViewModelFactory;

        WeakReferenceMessenger.Default.Register(this);
    }

    internal PortViewModel() : this(NullDialogService.Instance, NullGateway.Instance, _ => new()) { }



    public AvaloniaList<PortSummaryViewModel> Ports { get; init; } = [];


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
                AddPortSummaryViewModel(port);
            }
            else if (content.IsSelectedSerialFrom)
            {
                var from = content.SerialForm;
                var serialChannelDescriptor = SerialChannelDescriptor.Create(from.PortName, from.BaudRate, from.Parity, from.DataBits, from.StopBits);

                var port = _gateway.AddPort(serialChannelDescriptor);
                AddPortSummaryViewModel(port);
            }

            _dialogService.Close(dialogContent);
        }
        void OnCancel()
        {
            _dialogService.Close(dialogContent);
        }
    }


    private void AddPortSummaryViewModel(IPort port)
    {
        var vm = _portSummaryViewModelFactory(port);
        Ports.Add(vm);
    }
    void IRecipient<PortRemovedMessage>.Receive(PortRemovedMessage message)
    {
        var portVm = Ports.FirstOrDefault(x => x.Id == message.PortId);
        if (portVm is null) return;

        Ports.Remove(portVm);
    }
}


public sealed partial class PortSummaryViewModel : ViewModelBase
{
    private readonly IPort _port;
    private readonly IAliasService<PortId> _portAliasService;
    private readonly DeviceSummaryViewModelFactory _deviceSummaryViewModelFactory;


    public PortSummaryViewModel(IPort port, IAliasService<PortId> portAliasService, DeviceSummaryViewModelFactory deviceSummaryViewModelFactory)
    {
        _port = port;
        _portAliasService = portAliasService;
        _deviceSummaryViewModelFactory = deviceSummaryViewModelFactory;

        Sync();
        SyncDevices();
    }
    internal PortSummaryViewModel() : this(NullPort.Instance, AliasService<PortId>.Instance, _ => new()) { }


    public PortId Id => _port.Id;
    public AvaloniaList<DeviceSummaryViewModel> Devices { get; init; } = [];


    [ObservableProperty]
    public partial string DisplayName { get; set; } = null!;

    [ObservableProperty]
    public partial ChannelAddress ChannelAddress { get; set; } = null!;

    [ObservableProperty]
    public partial bool IsOpen { get; set; }

    [ObservableProperty]
    public partial bool IsExpanded { get; set; } = true;



    public void Sync()
    {
        IsOpen = _port.IsOpen;
        ChannelAddress = _port.ChannelAddress;
        DisplayName = _portAliasService.Get(Id) ?? ChannelAddress.ToString();
    }
    public void SyncDevices()
    {
        Devices.Clear();
        Devices.AddRange(_port.Devices.Values
            .Select(device => _deviceSummaryViewModelFactory(device)));
    }
}
public delegate PortSummaryViewModel PortSummaryViewModelFactory(IPort port);


public sealed partial class DeviceSummaryViewModel : ViewModelBase
{
    private readonly IDevice _device;
    private readonly IAliasService<DeviceId> _aliasService;

    public DeviceSummaryViewModel(IDevice device, IAliasService<DeviceId> deviceAliasService)
    {
        _device = device;
        _aliasService = deviceAliasService;

        Sync();
    }
    internal DeviceSummaryViewModel() : this(NullDevice.Instance, AliasService<DeviceId>.Instance) { }


    public DeviceId Id => _device.Id;

    [ObservableProperty]
    public partial string DisplayName { get; set; } = null!;

    [ObservableProperty]
    public partial bool IsEnabled { get; set; }

    [ObservableProperty]
    public partial ProtocolType ProtocolType { get; set; }


    public void Sync()
    {
        IsEnabled = _device.IsEnabled;
        ProtocolType = _device.ProtocolType;
        DisplayName = _aliasService.Get(Id) ?? $"{ProtocolType}{Id.ToString()[^4..]}";
    }
}
public delegate DeviceSummaryViewModel DeviceSummaryViewModelFactory(IDevice device);