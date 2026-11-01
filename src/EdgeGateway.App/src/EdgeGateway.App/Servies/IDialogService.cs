using EdgeGateway.App.Models;

namespace EdgeGateway.App.Servies;

public interface IDialogService
{
    void Show(DialogContent vm);
    void Close(DialogContent vm);
}