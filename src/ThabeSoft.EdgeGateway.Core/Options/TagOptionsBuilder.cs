using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Options;

internal sealed class TagOptionsBuilder : ITagOptionsBuilder
{
    private TagName? _name;
    private ITagDescriptor? _descriptor;


    public ITagOptionsBuilder WithDescriptor(ITagDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        _descriptor = descriptor;
        return this;
    }

    public ITagOptionsBuilder WithName(TagName name)
    {
        _name = name;
        return this;
    }

    public ITagOptionsBuilder WithTrigger(ITrigger trigger)
    {
        throw new NotImplementedException();
    }

    public TagOptions Build()
    {
        ArgumentNullException.ThrowIfNull(_name);
        ArgumentNullException.ThrowIfNull(_descriptor);

        return new()
        {
            Descriptor = _descriptor,
            Name = _name
        };
    }

    
}


public static class TagOptionsBuilderExtensions
{
    extension(ITagOptionsBuilder builder)
    {
        public ITagOptionsBuilder WithFixedIntervalTrigger(TimeSpan timeSpan) =>
            builder.WithTrigger(new FixedIntervalTrigger(timeSpan));

        public ITagOptionsBuilder WithFixedIntervalTrigger(double milliseconds) =>
           builder.WithFixedIntervalTrigger(TimeSpan.FromMilliseconds(milliseconds));
    }
}



internal sealed record class TagOptions
{
    public required TagName Name { get; init; }
    public required ITagDescriptor Descriptor { get; init; }
}