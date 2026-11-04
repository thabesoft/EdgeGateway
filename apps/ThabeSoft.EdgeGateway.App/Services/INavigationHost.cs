using ThabeSoft.EdgeGateway.App.Features;

namespace ThabeSoft.EdgeGateway.App.Services;


internal sealed class NavigationService(INavigationHost host)
{
    public void NavigateTo<T>() where T : PageViewModel
    {
        var pageType = typeof(T);
        var page = host.Pages.FirstOrDefault(x => x.GetType() == pageType);
        if (page is null || host.ActivePage?.GetType() == pageType) return;

        host.ActivePage?.OnNavigationLeave();
        host.ActivePage = page;
        host.ActivePage?.OnNavigationEnter();
    }
}

internal interface INavigationHost
{
    IReadOnlyList<PageViewModel> Pages { get; }
    PageViewModel? ActivePage { get; set; }
}