using ThabeSoft.EdgeGateway.App.ViewModels;

namespace ThabeSoft.EdgeGateway.App.Services;


public interface INavigationService
{
    void Navigate(Type type);
    void Navigate(ViewModelBase content);
}