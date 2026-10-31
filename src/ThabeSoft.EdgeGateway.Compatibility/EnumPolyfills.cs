#if NETSTANDARD2_0

namespace System;

public static class EnumPolyfills
{
    extension(Enum)
    {
        public static bool IsDefined<T>(T value) where T : struct, Enum
        {
            return Enum.IsDefined(typeof(T), value);
        }
    }
}

#endif