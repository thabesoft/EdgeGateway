using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Options;

public interface ITagOptionsBuilder
{
    ITagOptionsBuilder WithDescriptor(ITagDescriptor descriptor);
    ITagOptionsBuilder WithName(TagName name);
    ITagOptionsBuilder WithTrigger(ITrigger trigger);
}


public static class TagOptionsBuilderExtensions
{
    extension(ITagOptionsBuilder builder)
    {
        public ITagOptionsBuilder WithName(string tagName) =>
            builder.WithName(TagName.From(tagName));
    }
}
