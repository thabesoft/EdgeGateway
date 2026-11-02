#if NETSTANDARD2_0

using System.Diagnostics.CodeAnalysis;

namespace System;

public static class ObjectDisposedExceptionPolyfills
{
    extension(ObjectDisposedException)
    {
        public static void ThrowIf([DoesNotReturnIf(true)] bool condition, object instance)
        {
            if (condition)
                throw new ObjectDisposedException(instance.GetType().FullName);
        }
    }
}

#endif


