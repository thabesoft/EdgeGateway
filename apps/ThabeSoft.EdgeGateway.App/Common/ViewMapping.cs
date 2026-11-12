using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.EdgeGateway.App.Features;

namespace ThabeSoft.EdgeGateway.App.Common;


public sealed class ViewMapping
{
    public required Type View { get; init; }
    public required Type ViewModel { get; init; }


    public static ViewMapping Create<TView, TViewModel>()
        where TView : IDataContextProvider, new()
        where TViewModel : ObservableObject
    {
        return new()
        {
            View = typeof(TView),
            ViewModel = typeof(TViewModel)
        };
    }
}