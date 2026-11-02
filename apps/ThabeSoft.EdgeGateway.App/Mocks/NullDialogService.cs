using ThabeSoft.EdgeGateway.App.Models;
using ThabeSoft.EdgeGateway.App.Services;

namespace ThabeSoft.EdgeGateway.App.Mocks;

public sealed class NullDialogService : IDialogService
{
    private NullDialogService() { }
    public static NullDialogService Instance { get; } = new();


    public void Close(DialogContent vm) { }
    public void Show(DialogContent vm) { }
}