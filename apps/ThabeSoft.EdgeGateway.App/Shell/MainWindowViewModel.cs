using CommunityToolkit.Mvvm.ComponentModel;

namespace ThabeSoft.EdgeGateway.App.Shell;


public partial class MainWindowViewModel(MainViewModel mainViewModel) : ViewModelBase
{
    [ObservableProperty]
    public partial string Title { get; private set; } = "EdgeGateway";

    [ObservableProperty]
    public partial MainViewModel Content { get; private set; } = mainViewModel;
}