namespace ThabeSoft.EdgeGateway.App.ViewModels;

internal sealed partial class PortDetailViewModel(PortViewModel portViewModel) : ViewModelBase
{
    public PortViewModel Port => portViewModel;
}