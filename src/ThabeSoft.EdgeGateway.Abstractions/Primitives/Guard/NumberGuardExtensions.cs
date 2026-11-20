#if !NETSTANDARD2_0

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ThabeSoft.Guard;

public static class NumberGuardExtensions
{
    extension<T>(T param) where T : INumber<T>
    {
        /// <summary>
        /// 必须 > 0
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [DebuggerStepThrough]
        [StackTraceHidden]
        public T Positive(
            string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (param <= T.Zero)
                throw new ArgumentOutOfRangeException(paramName, param, errorMessage ?? "不可为负数");

            return param;
        }

        /// <summary>
        /// 不是负数
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [DebuggerStepThrough]
        [StackTraceHidden]
        public T InRange(
            T start,
            T stop,
            string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (start > stop)
                throw new ArgumentException($"start({start}) 不能大于 stop({stop})", nameof(start));

            if (param < start || param > stop)
                throw new ArgumentOutOfRangeException(paramName, param, errorMessage ?? $"超出范围. 只能在[{start}~{stop}] 之间");

            return param;
        }
    }

    extension(string param)
    {
        [DebuggerStepThrough]
        [StackTraceHidden]
        public T ToNumber<T>(string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
            where T : INumber<T>
        {
            if (!T.TryParse(param, null, out var result))
                throw new FormatException(errorMessage ?? $"无法将(\"{param}\")转换为({typeof(int)}) > {paramName}");

            return result;
        }
    }
}

#endif