using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;

namespace ThabeSoft.EdgeGateway.App.Services;


internal sealed class ClipboardService(IClassicDesktopStyleApplicationLifetime liftime)
{
    public void CopyToClipboard(string text) => liftime.MainWindow?.Clipboard?.SetTextAsync(text);
}
