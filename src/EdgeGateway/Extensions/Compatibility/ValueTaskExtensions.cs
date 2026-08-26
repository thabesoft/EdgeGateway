#if NETSTANDARD2_0

namespace System.Threading.Tasks;

internal static class SystemThreadingTasksExtensions
{
    extension(ValueTask)
    {
        public static ValueTask<T> FromResult<T>(T result)
        {
            return new(result);
        }

        public static ValueTask CompletedTask => default;
    }
}

#endif