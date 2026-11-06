#if NETSTANDARD2_0

namespace System;

public static class Int32PoluFills
{
    extension(int)
    {
        public static bool TryParse(ReadOnlySpan<char> s, out int result)
        {
            return int.TryParse(s.ToString(), out result);
        }
    }
}

#endif