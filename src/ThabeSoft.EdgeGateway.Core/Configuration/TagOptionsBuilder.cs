using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Configuration;

internal sealed class TagOptionsBuilder<T> : ITagOptionsConfigurator<T>
    where T : notnull
{
    private TagId? _name;
    private TagDescriptor<T>? _descriptor;
    private ITrigger? _trigger;


    public ITagOptionsConfigurator<T> WithDescriptor(TagDescriptor<T> descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        _descriptor = descriptor;
        return this;
    }

    public ITagOptionsConfigurator<T> WithName(TagId name)
    {
        _name = name;
        return this;
    }

    public ITagOptionsConfigurator<T> WithTrigger(ITrigger trigger)
    {
        _trigger = trigger;
        return this;
    }

    public TagOptions<T> Build()
    {
        ArgumentNullException.ThrowIfNull(_name);
        ArgumentNullException.ThrowIfNull(_descriptor);
        ArgumentNullException.ThrowIfNull(_trigger);

        return new()
        {
            Descriptor = _descriptor,
            Name = _name,
            Trigger = _trigger
        };
    }


}


public static class TagOptionsBuilderExtensions
{
    extension<T>(ITagOptionsConfigurator<T> builder) where T : notnull
    {
        public ITagOptionsConfigurator<T> WithFixedIntervalTrigger(TimeSpan timeSpan) =>
            builder.WithTrigger(new FixedIntervalTrigger(timeSpan));

        public ITagOptionsConfigurator<T> WithFixedIntervalTrigger(double milliseconds) =>
           builder.WithFixedIntervalTrigger(TimeSpan.FromMilliseconds(milliseconds));
    }
}



internal interface ITagOptions
{
    TagId Name { get; }
    ITrigger Trigger { get; }

    void RegisterTo(IPollingManager pollingManager);
}

internal sealed class TagOptions<T> : ITagOptions
    where T : notnull
{
    public required TagId Name { get; init; }
    public required TagDescriptor<T> Descriptor { get; init; }
    public required ITrigger Trigger { get; init; }

    void ITagOptions.RegisterTo(IPollingManager pollingManager) =>
        pollingManager.Register(Descriptor, Trigger);
}