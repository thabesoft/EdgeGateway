#if NETSTANDARD2_0

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System.IO;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

internal static class SystemExtensions
{
    extension(Enum)
    {
        public static bool IsDefined<T>(T value) where T : Enum
        {
            return Enum.IsDefined(typeof(T), value);
        }
    }
}

#endif