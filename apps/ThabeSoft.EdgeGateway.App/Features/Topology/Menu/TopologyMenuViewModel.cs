using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using ThabeSoft.EdgeGateway.App.Features.Gateway;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Messages;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.PubSub;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;


internal sealed class TopologyMenuViewModel : ObservableObject
{
    public readonly AvaloniaList<MenuItemViewModel> _items = [];

    public TopologyMenuViewModel(IGateway gateway, PortViewModelFactory portViewModelFactory, DeviceViewModelFactory deviceViewModelFactory, IEventSubscriber eventSubscriber)
    {
        // 初始化菜单
        _items = [.. gateway.Ports.Values.Select(x => CreatePortItem(portViewModelFactory.Create(x)))];

        eventSubscriber.Subscribe<PortAddedMessage>((e, _) =>
        {
            if (gateway.Ports.TryGetValue(e.PortId, out var port))
            {
                AddPort(portViewModelFactory.Create(port));
            }

            return ValueTask.CompletedTask;
        });
        eventSubscriber.Subscribe<PortRemovedMessage>((e, _) =>
        {
            RemovePort(e.PortId);
            return ValueTask.CompletedTask;
        });

        eventSubscriber.Subscribe<DeviceAddedMessage>((e, _) =>
        {
            if (gateway.Ports.TryGetValue(e.PortId, out var port) && port.Devices.TryGetValue(e.DeviceId, out var device))
            {
                AddDevice(e.PortId, deviceViewModelFactory.Create(device));
            }

            return ValueTask.CompletedTask;
        });
        eventSubscriber.Subscribe<DeviceRemovedMessage>((e, _) =>
        {
            RemoveDevice(e.PortId, e.DeviceId);
            return ValueTask.CompletedTask;
        });
    }

    public IAvaloniaReadOnlyList<MenuItemViewModel> Items => _items;



    public bool AddPort(PortViewModel port)
    {
        if (FindPortMenu(_items, port.Id) is not null)
            return false;

        var menuItem = CreatePortItem(port);

        Dispatcher.UIThread.Invoke(() => _items.Add(menuItem));
        return true;

    }
    public bool RemovePort(PortId portId)
    {
        var portMenu = FindPortMenu(_items, portId);
        if (portMenu is null) return false;

        return Dispatcher.UIThread.Invoke(() => _items.Remove(portMenu));
    }


    public bool AddDevice(PortId portId, DeviceViewModel device)
    {
        // 找到端口菜单
        var portMenu = FindPortMenu(_items, portId);
        if (portMenu is null) return false;

        // 找到端口下设备菜单
        var deviceMenu = FindDeviceMenu(portMenu.ChildIs, device.Id);
        if (deviceMenu is not null) return false;

        // 创建新的设备菜单
        var item = CreateDeviceItem(device);
        portMenu.AddChild(item);
        return true;
    }
    public bool RemoveDevice(PortId portId, DeviceId deviceId)
    {
        // 找到端口菜单
        var portMenu = FindPortMenu(_items, portId);
        if (portMenu is null) return false;

        // 找到端口下设备菜单
        var deviceMenu = FindDeviceMenu(portMenu.ChildIs, deviceId);
        if (deviceMenu is null) return false;

        // 删除菜单
        return portMenu.RemoveChild(deviceMenu);
    }



    private static MenuItemViewModel? FindPortMenu(IEnumerable<MenuItemViewModel> menus, PortId portId)
    {
        return menus.FirstOrDefault(x => x.PortContent?.Id == portId);
    }
    private static MenuItemViewModel? FindDeviceMenu(IEnumerable<MenuItemViewModel> menus, DeviceId deviceId)
    {
        return menus.FirstOrDefault(x => x.DeviceContent?.Id == deviceId);
    }

    private static MenuItemViewModel CreatePortItem(PortViewModel port)
    {
        var item = new MenuItemViewModel()
        {
            Header = port.Endpoint.ToString(),
            Content = port
        };

        item.AddChilds(port.Devices.Values.Select(CreateDeviceItem));
        return item;
    }
    private static MenuItemViewModel CreateDeviceItem(DeviceViewModel device)
    {
        return new()
        {
            Header = device.Id.ToString(),
            Content = device
        };
    }
}
