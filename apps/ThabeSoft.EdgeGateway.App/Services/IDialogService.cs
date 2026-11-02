using ThabeSoft.EdgeGateway.App.Models;

namespace ThabeSoft.EdgeGateway.App.Services;

public interface IDialogService
{
    void Show(DialogContent vm);
    void Close(DialogContent vm);
}