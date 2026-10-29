#if NETSTANDARD2_0

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System;
public static class ArgumentExceptionPolyfills
{
    extension(ArgumentException)
    {
        public static void ThrowIfNullOrWhiteSpace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (string.IsNullOrWhiteSpace(argument))
                throw new ArgumentException("The value cannot be an empty string or contain only whitespace.", paramName);
        }
    }
}

#endif