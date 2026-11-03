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

public static class ViewMappingExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddViewMapping<TView, TViewModel>(ServiceLifetime lifetime)
            where TView : IDataContextProvider, new()
            where TViewModel : ObservableObject
        {
            var viewModelType = typeof(TViewModel);

            if (viewModelType.IsAssignableTo(typeof(PageViewModel)))
            {
                descriptors.Add(ServiceDescriptor.Describe(typeof(PageViewModel), viewModelType, lifetime));
            }
            else
            {
                descriptors.Add(ServiceDescriptor.Describe(viewModelType, viewModelType, lifetime));
            }

            return descriptors.AddSingleton(ViewMapping.Create<TView, TViewModel>());
        }
    }
}