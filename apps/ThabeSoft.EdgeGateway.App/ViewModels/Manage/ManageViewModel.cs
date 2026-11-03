using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.App.Models;
using ThabeSoft.EdgeGateway.App.Services;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


internal partial class ManageViewModel(IDialogService dialogService, GatewayViewModel gatewayViewModel, ILogger<ManageViewModel> logger) : ViewModelBase
{
    public GatewayViewModel Gateway => gatewayViewModel;


    [ObservableProperty]
    public partial PortViewModel? SelectedPort { get; set; } = gatewayViewModel.Ports.FirstOrDefault();



    [RelayCommand]
    private void ShowAddPortDialog()
    {
        logger.LogDebug("显示添加端口弹窗");

        var content = new AddPortViewModel();
        DialogContent dialogContent = null!;
        dialogContent = DialogContent.OkCancel(content: content, okCallback: OnCreate, cancelCallback: OnCancel, "添加端口");

        dialogService.Show(dialogContent);


        void OnCreate()
        {
            var descriptor = content.Build();
            gatewayViewModel.AddPortCommand.Execute(descriptor);

            dialogService.Close(dialogContent);
        }

        void OnCancel()
        {
            dialogService.Close(dialogContent);
        }
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
}