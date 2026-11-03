using ThabeSoft.EdgeGateway.App.Features;

namespace ThabeSoft.EdgeGateway.App.Services;


internal sealed class PageNavigationService
{
    public Action<Type>? NavigationRequested { get; set; }

    public void RequestNavigation<T>() where T : PageViewModel
    {
        NavigationRequested?.Invoke(typeof(T));
    }
}