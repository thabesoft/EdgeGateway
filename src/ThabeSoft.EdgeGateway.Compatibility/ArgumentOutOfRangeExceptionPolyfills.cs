#if NETSTANDARD2_0

using System.Runtime.CompilerServices;

namespace System;

public static class ArgumentOutOfRangeExceptionPolyfills
{
    extension(ArgumentOutOfRangeException)
    {
        private static void OnThrowNegativeOrZero(object value, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be a non-negative and non-zero value.");

        public static void ThrowIfNegativeOrZero(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
            if (value <= 0) OnThrowNegativeOrZero(value, paramName);
        }
        public static void ThrowIfNegativeOrZero(double value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
            if (value <= 0) OnThrowNegativeOrZero(value, paramName);
        }




        private static void OnThrowIfZero(object value, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} ('{value}') must be a non-negative and non-zero value.");

        public static void ThrowIfZero(double value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
            if (value == 0) OnThrowIfZero(value, paramName);
        }
        public static void ThrowIfZero(TimeSpan value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
            if (value == TimeSpan.Zero) OnThrowIfZero(value, paramName);
        }
    }
}

#endif