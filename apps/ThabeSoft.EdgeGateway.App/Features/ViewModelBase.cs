using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ThabeSoft.EdgeGateway.App.Features;


public abstract class ViewModelBase : ObservableObject, INotifyDataErrorInfo
{
    private readonly Dictionary<string, string[]> _errors = [];


    public bool HasErrors => _errors.Count > 0;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName) || !_errors.TryGetValue(propertyName, out var list))
        {
            return Array.Empty<ValidationResult>();
        }

        return list.Select(errorMessage => new ValidationResult(errorMessage, [propertyName]));
    }



    protected bool TrySetPropertyError(string propertyName, params IReadOnlyCollection<string?>? errorMessages)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return false;

        var newErrors = errorMessages?.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray() ?? [];
        // 没有消息则清空错误
        if (newErrors.Length == 0)
            return TryClearPropertyError(propertyName);

        // 和之前的错误消息一样
        if (_errors.TryGetValue(propertyName, out var oldErrors) && oldErrors.SequenceEqual(newErrors))
            return false;

        _errors[propertyName] = newErrors!;
        ErrorsChanged?.Invoke(this, new(propertyName));
        return true;
    }


    protected bool TryClearPropertyError(string propertyName)
    {
        if (!_errors.Remove(propertyName)) return false;

        ErrorsChanged?.Invoke(this, new(propertyName));
        return true;
    }

    protected delegate bool Validator<T>(T value, [NotNullWhen(false)] out string? error);
}