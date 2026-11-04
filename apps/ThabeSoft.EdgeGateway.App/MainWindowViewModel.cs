using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SukiUI.Dialogs;
using SukiUI.Enums;
using SukiUI.Toasts;
using ThabeSoft.EdgeGateway.App.Features;
using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.App.Utilities;

namespace ThabeSoft.EdgeGateway.App;


internal partial class MainWindowViewModel(
        ISukiToastManager toastManager,
        ISukiDialogManager dialogManager,
        IEnumerable<PageViewModel> pageViewModels
    ) : ObservableObject, INavigationHost
{
    private readonly AvaloniaList<PageViewModel> _pages = [..pageViewModels
        .OrderBy(x => x.Index).ThenBy(x => x.DisplayName)];


    public ISukiToastManager ToastManager { get; } = toastManager;
    public ISukiDialogManager DialogManager { get; } = dialogManager;
    public IReadOnlyList<PageViewModel> Pages => _pages;


    [ObservableProperty]
    public partial PageViewModel? ActivePage { get; set; }



    [ObservableProperty]
    public partial bool AnimationsEnabled { get; set; }

    [ObservableProperty]
    public partial SukiBackgroundStyle BackgroundStyle { get; set; } = SukiBackgroundStyle.GradientSoft;

    [ObservableProperty]
    public partial string? CustomShaderFile { get; set; }

    [ObservableProperty]
    public partial bool ShowBottomBar { get; set; } = true;

    [ObservableProperty]
    public partial bool ShowTitleBar { get; set; } = true;

    [ObservableProperty]
    public partial bool TitleBarVisible { get; set; } = true;

    [ObservableProperty]
    public partial bool TransitionsEnabled { get; set; }

    [ObservableProperty]
    public partial bool WindowLocked { get; set; }


    [RelayCommand]
    private static void OpenUrl(string url)
    {
        UrlUtilities.OpenUrl(url);
    }
}