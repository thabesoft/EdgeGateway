using CommunityToolkit.Mvvm.ComponentModel;

namespace EdgeGateway.App.ViewModels;


public partial class MainWindowViewModel(MainViewModel mainViewModel) : ViewModelBase
{
    [ObservableProperty]
    public partial string Title { get; private set; } = "EdgeGateway.App";

    [ObservableProperty]
    public partial MainViewModel Content { get; private set; } = mainViewModel;
}