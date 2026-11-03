using Avalonia.Logging;
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
    public partial object? SelectedItem { get; set; }

    [ObservableProperty]
    public partial PortViewModel? SelectedPort { get; private set; }
    [ObservableProperty]
    public partial DeviceViewModel? SelectedDevice { get; private set; }



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

    partial void OnSelectedItemChanged(object? value)
    {
        if (value is PortViewModel portVm)
        {
            SelectedPort = portVm;
            SelectedDevice = null;
            return;
        }

        if (value is DeviceViewModel deviceVm)
        {
            SelectedItem = null;
            SelectedDevice = deviceVm;
            return;
        }
    }
}