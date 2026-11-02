using ThabeSoft.EdgeGateway.App.Dialog;

namespace ThabeSoft.EdgeGateway.App.Mocks;

public sealed class NullDialogService : IDialogService
{
    private NullDialogService() { }
    public static NullDialogService Instance { get; } = new();


    public void Close(DialogContent vm) { }
    public void Show(DialogContent vm) { }
}