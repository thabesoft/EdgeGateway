using CommunityToolkit.Mvvm.ComponentModel;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


public partial class MainWindowViewModel(MainViewModel mainViewModel) : ViewModelBase
{
    [ObservableProperty]
    public partial string Title { get; private set; } = "EdgeGateway";

    [ObservableProperty]
    public partial MainViewModel Content { get; private set; } = mainViewModel;
}