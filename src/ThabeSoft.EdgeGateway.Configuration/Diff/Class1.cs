using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.Configuration.Diff;


public sealed class PortDiffResult
{
    public required IReadOnlyList<PortDescriptor> Added { get; init; }
    public required IReadOnlyList<PortDescriptor> Removed { get; init; }
    public required IReadOnlyList<PortDescriptor> Unchanged { get; init; }
    public required IReadOnlyList<PortWithChildDiff> Updated { get; init; }
}

public sealed class PortWithChildDiff
{
    public required PortDescriptor Old { get; init; }
    public required PortDescriptor New { get; init; }
    public required DeviceDiffResult DeviceDiff { get; init; }
}

public sealed class DeviceDiffResult
{
    public required IReadOnlyList<DeviceDescriptor> Added { get; init; }
    public required IReadOnlyList<DeviceDescriptor> Removed { get; init; }
    public required IReadOnlyList<DeviceWithChildDiff> Updated { get; init; }
    public required IReadOnlyList<DeviceDescriptor> Unchanged { get; init; }
}

public sealed class DeviceWithChildDiff
{
    public required DeviceDescriptor Old { get; init; }
    public required DeviceDescriptor New { get; init; }
    public required SubscriptionDiffResult SubscriptionDiff { get; init; }
}

public sealed class SubscriptionDiffResult
{
    public required IReadOnlyList<ISubscriptionDescriptor> Added { get; init; }
    public required IReadOnlyList<ISubscriptionDescriptor> Removed { get; init; }
    public required IReadOnlyList<(ISubscriptionDescriptor Old, ISubscriptionDescriptor New)> Updated { get; init; }
    public required IReadOnlyList<ISubscriptionDescriptor> Unchanged { get; init; }
}