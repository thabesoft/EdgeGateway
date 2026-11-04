using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Options;

internal sealed class TagOptionsBuilder<T> : ITagOptionsBuilder<T>
    where T : notnull
{
    private TagName? _name;
    private ITagDescriptor<T>? _descriptor;
    private ITrigger? _trigger;


    public ITagOptionsBuilder<T> WithDescriptor(ITagDescriptor<T> descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        _descriptor = descriptor;
        return this;
    }

    public ITagOptionsBuilder<T> WithName(TagName name)
    {
        _name = name;
        return this;
    }

    public ITagOptionsBuilder<T> WithTrigger(ITrigger trigger)
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
    extension<T>(ITagOptionsBuilder<T> builder) where T : notnull
    {
        public ITagOptionsBuilder<T> WithFixedIntervalTrigger(TimeSpan timeSpan) =>
            builder.WithTrigger(new FixedIntervalTrigger(timeSpan));

        public ITagOptionsBuilder<T> WithFixedIntervalTrigger(double milliseconds) =>
           builder.WithFixedIntervalTrigger(TimeSpan.FromMilliseconds(milliseconds));
    }
}



internal interface ITagOptions
{
    TagName Name { get; }
    ITagDescriptor Descriptor { get; }
    ITrigger Trigger { get; }

    void RegisterTo(IPollingManager pollingManager);
}

internal sealed class TagOptions<T> : ITagOptions
    where T : notnull
{
    public required TagName Name { get; init; }
    public required ITagDescriptor<T> Descriptor { get; init; }
    public required ITrigger Trigger { get; init; }

    ITagDescriptor ITagOptions.Descriptor => Descriptor;

    void ITagOptions.RegisterTo(IPollingManager pollingManager) =>
        pollingManager.Register(Descriptor, Trigger);
}