#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
using SukiUI.Toasts;

namespace ThabeSoft.EdgeGateway.App.Services;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


internal static class SukiToastManagerExtensions
{
    extension(ISukiToastManager sukiToastManager)
    {
        public async ValueTask QueueWithCatchExceptionAsync(Func<ValueTask> action, string title)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                sukiToastManager.CreateToast()
                    .OfType(Avalonia.Controls.Notifications.NotificationType.Error)
                    .WithTitle(title)
                    .WithContent(ex.Message)
                    .Queue();
            }
        }
    }
}