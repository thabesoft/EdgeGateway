using ThabeSoft.EdgeGateway.App.Models;

namespace ThabeSoft.EdgeGateway.App.Services;

public interface IDialogService
{
    void Show(DialogContent vm);
    void Close(DialogContent vm);
}

public sealed class NullDialogService : IDialogService
{
    private NullDialogService() { }
    public static NullDialogService Instance { get; } = new();


    public void Close(DialogContent vm) { }
    public void Show(DialogContent vm) { }
}