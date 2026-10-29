#if NETSTANDARD2_0

using System.Runtime.CompilerServices;

namespace System;

public static class ArgumentOutOfRangeExceptionPolyfills
{
    extension(ArgumentOutOfRangeException)
    {
        private static void ThrowNegativeOrZero(object value, string? paramName)
        {
            throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be a non-negative and non-zero value.");
        }

        public static void ThrowIfNegativeOrZero(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
            if (value <= 0) ThrowNegativeOrZero(value, paramName);
        }
    }
}

#endif