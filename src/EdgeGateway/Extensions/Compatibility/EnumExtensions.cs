#if NETSTANDARD2_0

namespace System;

public static class SystemExtensions
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