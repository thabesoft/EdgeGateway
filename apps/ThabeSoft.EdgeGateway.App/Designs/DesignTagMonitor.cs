using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.App.Designs;

public sealed class DesignTagMonitor<T>(ITagDescriptor<T> descriptor) : ITagMonitor<T>
    where T : notnull
{
    public event Action<T>? Updated;
    public TagName Name => descriptor.Name;
    public IDataAddress Address => descriptor.Address;


    public T? Value { get; } = default;
    public DateTimeOffset UpdateTime { get; } = DateTimeOffset.MinValue;
    public TagValueState State { get; } = TagValueState.Bad;

}
