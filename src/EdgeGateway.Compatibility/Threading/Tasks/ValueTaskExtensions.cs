#if NETSTANDARD2_0

namespace System.Threading.Tasks;

public static class ValueTaskExtensions
{
    extension(ValueTask)
    {
        public static ValueTask CompletedTask => default;

        public static ValueTask<T> FromResult<T>(T result)
        {
            return new(result);
        }
    }
}

#endif