#if NETSTANDARD2_0

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System.Threading.Tasks;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

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