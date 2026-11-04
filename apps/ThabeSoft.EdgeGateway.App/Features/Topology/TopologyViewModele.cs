using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.App.ViewModels;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;


internal sealed partial class TopologyViewModel(
        ISukiDialogManager sukiDialog,
        ISukiToastManager sukiToastManager,
        GatewayViewModel gatewayViewModel
    ) : PageViewModel("拓扑", MaterialIconKind.Dog)
{
    private readonly AvaloniaList<MenuItemViewModel> _items = [];

    public GatewayViewModel Gateway => gatewayViewModel;
    public IAvaloniaReadOnlyList<MenuItemViewModel> MenuItemsSource => _items;


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
    private void ShowAddPortDialog()
    {
        sukiDialog.CreateDialog()
            .WithTitle("创建端口")
            .WithViewModel(dialog => new AddPortViewModel(dialog, sukiToastManager, gatewayViewModel), false)
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

        var nextItem = GetNextItem([.. Gateway.Ports], SelectedPort);
        await Gateway.RemovePortCommand.ExecuteAsync(SelectedPort.ChannelAddress);
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


    public override void OnNavigationEnter()
    {
        var ports = gatewayViewModel.Ports
            .Select(port => new MenuItemViewModel()
            {
                Header = port.ChannelAddress.ToString(),
                Content = port,
                ChildItemsSource = new AvaloniaList<MenuItemViewModel>(port.Devices
                     .Select(device => new MenuItemViewModel()
                     {
                         Header = device.Id.ToString(),
                         Content = device
                     }))
            });

        _items.Clear();
        _items.AddRange(ports);
        SelectedMenu = _items.FirstOrDefault();
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