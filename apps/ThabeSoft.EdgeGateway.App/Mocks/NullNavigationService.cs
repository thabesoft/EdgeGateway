using ThabeSoft.EdgeGateway.App.Services;
using ThabeSoft.EdgeGateway.App.ViewModels;

namespace ThabeSoft.EdgeGateway.App.Mocks;

public sealed class NullNavigationService : INavigationService
{
    private NullNavigationService() { }
    public static NullNavigationService Instance { get; } = new();


    public void Navigate(Type type) { }
    public void Navigate(ViewModelBase content) { }
}