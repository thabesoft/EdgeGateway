#if NETSTANDARD2_0

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class ExceptionExtensions
{
    extension(ArgumentException)
    {
        /// <summary>
        /// Throws an System.ArgumentException if argument is null or whitespace.
        /// </summary>
        /// <param name="argument">The string argument to validate as non-null and non-whitespace.</param>
        /// <param name="paramName">
        /// The name of the parameter with which argument corresponds. If you omit this parameter,
        /// the name of argument is used.
        /// </param>
        /// <exception cref="ArgumentException">argument is null or whitespace.</exception>
        public static void ThrowIfNullOrWhiteSpace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
#pragma warning disable CS8777 // 退出时，参数必须具有非 null 值。
            if (!string.IsNullOrWhiteSpace(argument)) return;
#pragma warning restore CS8777 // 退出时，参数必须具有非 null 值。
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);
        }
    }

    extension(ArgumentNullException)
    {
        /// <summary>
        /// Throws an System.ArgumentNullException if argument is null.
        /// </summary>
        /// <param name="argument">The reference type argument to validate as non-null.</param>
        /// <param name="paramName">
        /// The name of the parameter with which argument corresponds. If you omit this parameter,
        /// the name of argument is used.
        /// </param>
        /// <exception cref="ArgumentNullException">argument is null.</exception>
        public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (argument is not null) return;
            throw new ArgumentNullException(paramName);
        }
    }
}

#endif