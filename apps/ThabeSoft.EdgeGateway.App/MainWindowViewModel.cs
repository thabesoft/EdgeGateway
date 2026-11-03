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


internal partial class MainWindowViewModel : ObservableObject
{
    public MainWindowViewModel(
            ISukiToastManager toastManager, 
            ISukiDialogManager dialogManager,
            PageNavigationService pageNavigationService,
            IEnumerable<PageViewModel> pageViewModels
        )
    {
        ToastManager = toastManager;
        DialogManager = dialogManager;


        Pages = new AvaloniaList<PageViewModel>(pageViewModels.OrderBy(x => x.Index).ThenBy(x => x.DisplayName));
        pageNavigationService.NavigationRequested += pageType =>
        {
            var page = Pages.FirstOrDefault(x => x.GetType() == pageType);
            if (page is null || ActivePage?.GetType() == pageType) return;
            ActivePage = page;
        };
    }

    public ISukiToastManager ToastManager { get; }
    public ISukiDialogManager DialogManager { get; }



    public IAvaloniaReadOnlyList<PageViewModel> Pages { get; }

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