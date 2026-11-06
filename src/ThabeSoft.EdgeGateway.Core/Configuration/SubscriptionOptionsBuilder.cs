using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Polling;

namespace ThabeSoft.EdgeGateway.Configuration;

internal sealed class SubscriptionOptionsBuilder<T> : ISubscriptionOptionsConfigurator<T>
    where T : notnull
{
    private SubscriptionDescriptor<T>? _descriptor;
    private List<Action<T>> _callbacks = [];


    public ISubscriptionOptionsConfigurator<T> WithDescriptor(SubscriptionDescriptor<T> descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        _descriptor = descriptor;
        return this;
    }

    public ISubscriptionOptionsConfigurator<T> AddTagCallback(Action<T> tagDescriptor)
    {
        _callbacks.Add(tagDescriptor);
        return this;
    }


    public SubscriptionOptions<T> Build()
    {
        ArgumentNullException.ThrowIfNull(_descriptor);

        return new()
        {
            Descriptor = _descriptor,
            TagUpdateHandlers = [.. _callbacks]
        };
    }

    
}

public interface ISubscriptionOptions
{
    ValueTask AddSubscriptionAsync(IDevice device);
}

internal sealed class SubscriptionOptions<T> : ISubscriptionOptions
    where T : notnull
{
    public required SubscriptionDescriptor<T> Descriptor { get; init; }

    public required IReadOnlyList<Action<T>> TagUpdateHandlers { get; init; }


    public async ValueTask AddSubscriptionAsync(IDevice device)
    {
        var subscription = await device.AddSubscriptionAsync(Descriptor);

        foreach (var i in TagUpdateHandlers)
        {
            subscription.Tag.Subscribe(i);
        }
    }
}