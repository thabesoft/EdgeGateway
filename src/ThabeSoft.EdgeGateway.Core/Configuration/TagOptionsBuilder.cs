using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Configuration;

internal sealed class TagOptionsBuilder<T> : ITagOptionsConfigurator<T>
    where T : notnull
{
    private TagDescriptor<T>? _descriptor;
    private ITriggerDescriptor? _triggerDescriptor;


    public ITagOptionsConfigurator<T> WithDescriptor(TagDescriptor<T> descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        _descriptor = descriptor;
        return this;
    }
    public ITagOptionsConfigurator<T> WithTrigger(ITriggerDescriptor triggerDescriptor)
    {
        _triggerDescriptor = triggerDescriptor;
        return this;
    }

    public TagOptions<T> Build()
    {
        ArgumentNullException.ThrowIfNull(_descriptor);
        ArgumentNullException.ThrowIfNull(_triggerDescriptor);

        return new()
        {
            Descriptor = _descriptor,
            TriggerDescriptor = _triggerDescriptor
        };
    }


}


public static class TagOptionsBuilderExtensions
{
    extension<T>(ITagOptionsConfigurator<T> builder) where T : notnull
    {
        public ITagOptionsConfigurator<T> WithFixedIntervalTrigger(TimeSpan timeSpan) =>
            builder.WithTrigger(FixedIntervalTriggerDescriptor.From(timeSpan));

        public ITagOptionsConfigurator<T> WithFixedIntervalTrigger(double milliseconds) =>
           builder.WithFixedIntervalTrigger(TimeSpan.FromMilliseconds(milliseconds));
    }
}



internal interface ITagOptions
{
    ITriggerDescriptor TriggerDescriptor { get; }
}

internal sealed class TagOptions<T> : ITagOptions
    where T : notnull
{
    public required TagDescriptor<T> Descriptor { get; init; }
    public required ITriggerDescriptor TriggerDescriptor { get; init; }
}