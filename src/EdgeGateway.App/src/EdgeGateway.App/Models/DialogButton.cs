using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace EdgeGateway.App.Models;

public sealed class DialogButton
{
    public required string Content { get; set; }
    public required DialogButtonType Type { get; set; }
    public required ICommand OnClickCommand { get; init; }


    private DialogButton() { }
    public static DialogButton Create(Action callback, DialogButtonType type, string content)
    {
        ArgumentNullException.ThrowIfNull(callback);
        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type), "不支持的按钮类型");
        ArgumentException.ThrowIfNullOrWhiteSpace(content);


        return new DialogButton
        {
            Content = content,
            Type = type,
            OnClickCommand = new RelayCommand(()=>
            {
                try
                {
                    callback();
                }
                catch(Exception ex)
                {
                    
                }
            })
        };
    }
    public static DialogButton Create(Func<Task> callback, DialogButtonType type, string content)
    {
        ArgumentNullException.ThrowIfNull(callback);
        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type), "不支持的按钮类型");
        ArgumentException.ThrowIfNullOrWhiteSpace(content);


        return new DialogButton
        {
            Content = content,
            Type = type,
            OnClickCommand = new AsyncRelayCommand(async()=>
            {
                try
                {
                    await callback();
                }
                catch (Exception ex)
                {

                }
            })
        };
    }



    public static DialogButton Normal(Action callback, string content) =>
        Create(callback, DialogButtonType.Normal, content);
    public static DialogButton Danger(Action callback, string content) =>
        Create(callback, DialogButtonType.Danger, content);
    public static DialogButton Confirm(Action callback, string content = "确认") =>
        Create(callback, DialogButtonType.Confirm, content);
    public static DialogButton Cancel(Action callback, string content = "取消") =>
        Create(callback, DialogButtonType.Cancel, content);
}