using EdgeGateway.App.ViewModels;

namespace EdgeGateway.App.Models;


public sealed class DialogContent : ViewModelBase
{
    public string? Title { get; init; }
    public required ViewModelBase Content { get; init; }
    public required DialogButton[] Buttons { get; init; }


    public static DialogContent OkCancel(ViewModelBase content, Action okCallback, Action cancelCallback, string? title = null)
    {
        return new DialogContent
        {
            Title = title,
            Content = content,
            Buttons = [DialogButton.Confirm(okCallback), DialogButton.Cancel(cancelCallback)]
        };
    }
}