using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace ThabeSoft.EdgeGateway.App.Common;


public sealed class ViewFactory(IServiceProvider serviceProvider, IEnumerable<ViewMapping> viewMappings)
{
    private readonly FrozenDictionary<Type, Type> _vmToViewMap = viewMappings
        .DistinctBy(x => x.ViewModel)
        .ToFrozenDictionary(x => x.ViewModel, v => v.View);

    public bool TryCreateView(Type viewModelType, [NotNullWhen(true)] out Control? view)
    {
        var viewModel = serviceProvider.GetRequiredService(viewModelType);

        return TryCreateView(viewModel, out view);
    }
    public bool TryCreateView(object? viewModel, [NotNullWhen(true)] out Control? view)
    {
        view = null;

        if (viewModel == null)
        {
            return false;
        }

        var viewModelType = viewModel.GetType();

        if (_vmToViewMap.TryGetValue(viewModelType, out var viewType))
        {
            view = Activator.CreateInstance(viewType) as Control;
            view?.DataContext = viewModel;
        }

        return view != null;
    }
    public Control CreateView<TViewModel>() where TViewModel : ObservableObject
    {
        var viewModelType = typeof(TViewModel);

        if (TryCreateView(viewModelType, out var view))
        {
            return view;
        }

        throw new InvalidOperationException();
    }
}