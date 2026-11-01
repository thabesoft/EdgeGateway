using Avalonia.Controls;
using Avalonia.Controls.Templates;
using EdgeGateway.App.Models;
using EdgeGateway.App.ViewModels;
using Microsoft.Extensions.Logging;
using System.Collections.Frozen;

namespace EdgeGateway.App.DataTemplates;


internal sealed class ViewModelTmpleate(
        IServiceProvider serviceProvider,
        IEnumerable<ViewMapping> viewMappings,
        ILogger<ViewModelTmpleate> logger
    ) : IDataTemplate
{
    // Key: ViewModel  Value:View
    private readonly FrozenDictionary<Type, Type> _mappings = viewMappings.ToFrozenDictionary(k => k.ViewModel, v => v.View);

    public bool Match(object? data)
    {
        if (data is ViewModelBase) return true;
        if (data is not Type t) return false;

        return typeof(ViewModelBase).IsAssignableFrom(t);
    }

    public Control? Build(object? param)
    {
        if(param is ViewModelBase viewModel)
        {
            return Build(viewModel);
        }

        if (param is Type viewModelType)
        {
            var tmp = serviceProvider.GetService(viewModelType) as ViewModelBase;
            if (tmp is not null) return Build(tmp);

            logger.LogError("无法获取视图模型实例");
            return null;
        }

        logger.LogError("无法构建视图, 未知的参数");
        return null;
    }

    public Control? Build(ViewModelBase vm)
    {
        var vmType = vm.GetType();

        if (!_mappings.TryGetValue(vmType, out var viewType))
        {
            logger.LogError("未找到视图模型 {viewModelType} 对应的视图类型", vmType.Name);
            return null;
        }

        if (Activator.CreateInstance(viewType) is not Control view)
        {
            logger.LogError("无法创建视图 {viewType}", viewType.Name);
            return null;
        }

        view.DataContext = vm;
        return view;
    }
}
