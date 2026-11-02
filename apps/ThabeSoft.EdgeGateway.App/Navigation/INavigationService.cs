namespace ThabeSoft.EdgeGateway.App.Navigation;


public interface INavigationService
{
    void Navigate(Type type);
    void Navigate(ViewModelBase content);
}