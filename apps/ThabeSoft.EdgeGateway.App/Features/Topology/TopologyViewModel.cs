using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.App.Features.Gateway;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.Lifecycles;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;


internal sealed partial class TopologyViewModel : PageViewModel
{
    private readonly MenuViewModel _menu;
    private readonly ISukiDialogManager _sukiDialogManager;
    private readonly ISukiToastManager _sukiToastManager;
    private readonly IGateway _gateway;
    private readonly ISerialPortNamesProvider _serialPortNamesProvider;
    private readonly GatewayViewModel _gatewayViewModel;


    public TopologyViewModel(
            ISukiDialogManager sukiDialogManager,
            ISukiToastManager sukiToastManager,
            IGateway gateway,
            ISerialPortNamesProvider serialPortNamesProvider,
            IEventSubscriber eventSubscriber,
            GatewayViewModel gatewayViewModel
    ) : base("拓扑", MaterialIconKind.Dog)
    {
        _sukiDialogManager = sukiDialogManager;
        _sukiToastManager = sukiToastManager;
        _gateway = gateway;
        _serialPortNamesProvider = serialPortNamesProvider;
        _gatewayViewModel = gatewayViewModel;

        _menu = new MenuViewModel(gatewayViewModel, eventSubscriber);
    }

    public GatewayViewModel Gateway => _gatewayViewModel;
    public IAvaloniaReadOnlyList<MenuItemViewModel> MenuItemsSource => _menu.Items;


    [ObservableProperty]
    public partial MenuItemViewModel? SelectedMenu { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSelecteedPort))]
    public partial PortViewModel? SelectedPort { get; set; }
    public bool IsSelecteedPort => SelectedPort is not null;


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSelectedDevice))]
    public partial DeviceViewModel? SelectedDevice { get; set; }
    public bool IsSelectedDevice => SelectedDevice is not null;



    [RelayCommand]
    private async Task ChangeGatewayAsync()
    {
        await _sukiToastManager.ExecuteWithToastAsync(async _ =>
        {
            if (_gateway.IsRunning)
            {
                await _gatewayViewModel.StopCommand.ExecuteAsync(null);
            }
            else
            {
                await _gatewayViewModel.StartCommand.ExecuteAsync(null);
            }
        }, "切换网关状态");
    }


    [RelayCommand]
    private void ShowAddPortDialog()
    {
        _sukiDialogManager.CreateDialog()
            .WithTitle("创建端口")
            .WithViewModel(dialog => new AddPortViewModel(dialog, _sukiToastManager, _gateway, _serialPortNamesProvider), false)
            .WithActionButton("取消", dialog => dialog.Dismiss(), false, "Flat")
            .WithActionButton("创建", dialog =>
            {
                if (dialog.Content is not AddPortViewModel vm) return;
                vm.CreateCommand.Execute(null);

            }, false, "Primary")
            .TryShow();
    }

    [RelayCommand]
    private async Task RemoveSelectedPortAsync()
    {
        if (SelectedPort is null) return;

        var nextItem = GetNextItem([.. Gateway.Ports.Values], SelectedPort);
        await _gateway.RemovePortAsync(SelectedPort.Id);

        SelectedPort = nextItem;


        static PortViewModel? GetNextItem(PortViewModel[] items, PortViewModel item)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i]?.Equals(item) != true) continue;

                // 后面还有一个元素
                if (i + 1 < items.Length)
                    return items[i + 1];

                // 当前是最后一个元素, 尝试获取前一个
                if (i + 1 >= items.Length && i >= 2)
                    return items[i - 2];

                // 前后都没有元素
                return default;
            }

            // 集中中不存在指定目标, 最后一个
            return items.LastOrDefault();
        }
    }

    partial void OnSelectedMenuChanged(MenuItemViewModel? value)
    {
        if (value is null) return;

        if (value.Content is PortViewModel port)
        {
            SelectedPort = port;
            SelectedDevice = null;

            return;
        }

        if (value.Content is DeviceViewModel device)
        {
            SelectedDevice = device;
            SelectedPort = null;
            return;
        }
    }
}