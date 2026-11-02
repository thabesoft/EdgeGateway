namespace ThabeSoft.EdgeGateway.App.Dialog;

public interface IDialogService
{
    void Show(DialogContent vm);
    void Close(DialogContent vm);
}