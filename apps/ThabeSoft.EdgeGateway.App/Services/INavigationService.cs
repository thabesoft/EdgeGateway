using ThabeSoft.EdgeGateway.App.ViewModels;

namespace ThabeSoft.EdgeGateway.App.Services;


public interface INavigationService
{
    void Navigate(Type type);
    void Navigate(ViewModelBase content);
}

public sealed class NullNavigationService : INavigationService
{
    private NullNavigationService() { }
    public static NullNavigationService Instance { get; } = new();


    public void Navigate(Type type) { }
    public void Navigate(ViewModelBase content) { }
}