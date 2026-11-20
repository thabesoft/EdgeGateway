using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ThabeSoft.Primitives.Guard;


public static class TimeSpanGuardExtensions
{
    extension(TimeSpan param)
    {
        /// <summary>
        /// 必须 > 0s
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [DebuggerStepThrough]
        public TimeSpan Positive(
            string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (param <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(paramName, param, errorMessage ?? "必须大于0s");

            return param;
        }

        /// <summary>
        /// 至少达到目标值
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [DebuggerStepThrough]
        public TimeSpan AtLeast(TimeSpan target,
            string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (param < target)
                throw new ArgumentOutOfRangeException(paramName, param, errorMessage ?? $"至少达到目标值: {target}");

            return param;
        }
    }

    extension(string param)
    {
        [DebuggerStepThrough]
        public TimeSpan ToTimeSpan(string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (!TimeSpan.TryParse(param, out var result))
                throw new FormatException(errorMessage ?? $"无法将(\"{param}\")转换为({typeof(TimeSpan)}) > {paramName}");

            return result;
        }
    }
}