using EdgeGateway.App.ViewModels;

namespace EdgeGateway.App.Servies;


public interface INavigationService
{
    void Navigate(Type type);
    void Navigate(ViewModelBase content);
}