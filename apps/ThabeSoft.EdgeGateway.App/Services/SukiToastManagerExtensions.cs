#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
using Avalonia;
using Avalonia.Controls.Notifications;
using SukiUI.Toasts;

namespace ThabeSoft.EdgeGateway.App.Services;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


internal static class SukiToastManagerExtensions
{
    extension(ISukiToastManager sukiToastManager)
    {
        public async ValueTask<bool> ExecuteWithToastAsync(Func<CancellationToken, ValueTask> action, string title, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(timeout);

            try
            {
                await action(cancellationToken);
                return true;
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                if (cancellationToken.IsCancellationRequested)
                    sukiToastManager.ShowToast(NotificationType.Warning, title, "操作已取消");
                else
                    sukiToastManager.ShowToast(NotificationType.Error, title, "操作超时");

                return false;
            }
            catch (TimeoutException)
            {
                sukiToastManager.ShowToast(NotificationType.Error, title, "操作超时");
                return false;
            }
            catch (Exception ex)
            {
                sukiToastManager.ShowToast(NotificationType.Error, title, ex.Message);
                return false;
            }
        }

        public ValueTask<bool> ExecuteWithToastAsync(Func<CancellationToken, ValueTask> action, string title, CancellationToken cancellationToken = default)
            => sukiToastManager.ExecuteWithToastAsync(action, title, Timeout.InfiniteTimeSpan, cancellationToken);



        private void ShowToast(NotificationType type, string title, string content)
        {
            Application.Current?.Dispatcher.Post(() =>
            {
                sukiToastManager
                    .CreateToast()
                    .OfType(type)
                    .WithTitle(title)
                    .WithContent(content)
                    .Queue();
            });
        }
    }
}