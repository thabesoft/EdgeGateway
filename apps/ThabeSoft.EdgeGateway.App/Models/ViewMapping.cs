using Avalonia;
using ThabeSoft.EdgeGateway.App.ViewModels;

namespace ThabeSoft.EdgeGateway.App.Models;


public sealed class ViewMapping
{
    public required Type View { get; init; }
    public required Type ViewModel { get; init; }


    public static ViewMapping Create<TViewModel, TView>()
        where TViewModel : ViewModelBase
        where TView : IDataContextProvider, new()
    {
        return new()
        {
            View = typeof(TView),
            ViewModel = typeof(TViewModel)
        };
    }
}