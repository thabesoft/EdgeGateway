using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Lifecycles.Collections;


public interface ILifecycleBatchExecutorFactory
{
    ILifecycleBatchExecutor Create(LifecycleBatchExecutorDescriptor descriptor);
}


public record class LifecycleBatchExecutorDescriptor
{
    public int Concurrency { get; init => field = value.Positive(); } = 3;
    public TimeSpan ItemStartTimeout { get; init => field = value.Positive(); } = TimeSpan.FromMinutes(5);
    public TimeSpan ItemStopTimeout { get; init => field = value.Positive(); } = TimeSpan.FromMinutes(5);
    public TimeSpan ItemDisposeTimeout { get; init => field = value.Positive(); } = TimeSpan.FromMinutes(5);
}