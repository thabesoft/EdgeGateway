using CommunityToolkit.Mvvm.Input;
using ThabeSoft.EdgeGateway.App.Mocks;
using ThabeSoft.EdgeGateway.App.Models;
using ThabeSoft.EdgeGateway.App.Services;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


internal partial class PortPageViewModel(IDialogService dialogService, GatewayViewModel gatewayViewModel) : ViewModelBase
{
    public GatewayViewModel Gateway => gatewayViewModel;

    public PortPageViewModel() : this(NullDialogService.Instance, new()) { }


    [RelayCommand]
    private void AddPort()
    {
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
}