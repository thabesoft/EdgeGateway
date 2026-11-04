using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Options;

public interface ITagOptionsBuilder<T> where T : notnull
{
    ITagOptionsBuilder<T> WithDescriptor(ITagDescriptor<T> descriptor);
    ITagOptionsBuilder<T> WithTrigger(ITrigger trigger);
}