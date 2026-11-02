using CommunityToolkit.Mvvm.ComponentModel;
using ThabeSoft.EdgeGateway.App.Models;
using ThabeSoft.EdgeGateway.App.Services;

namespace ThabeSoft.EdgeGateway.App.ViewModels;


public partial class MainViewModel : IDialogService
{
    private readonly List<DialogContent> _dialogs = [];

    /// <summary>
    /// 弹窗内容
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsShowDialog))]
    public partial DialogContent? Dialog { get; set; }

    /// <summary>
    /// 是否显示弹窗
    /// </summary>
    public bool IsShowDialog => Dialog is not null;



    public void Show(DialogContent vm)
    {
        _dialogs.Remove(vm);

        Dialog = vm;
        _dialogs.Add(vm);

    }
    public void Close(DialogContent vm)
    {
        _dialogs.Remove(vm);
        Dialog = _dialogs.LastOrDefault();
    }
}