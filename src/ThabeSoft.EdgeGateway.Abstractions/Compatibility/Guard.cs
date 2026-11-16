using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.CompilerServices;

namespace ThabeSoft.EdgeGateway.Compatibility;


public static class Guard
{
    public static ImmutableArray<T> ToImmutableArrayOrEmpty<T>(this IEnumerable<T> values)
    {
        return values?.Where(x => x is not null).ToImmutableArray() ?? [];
    }

    // string
    extension(string param)
    {
        [DebuggerStepThrough] //StackTraceHidden
        public T ToEnum<T>(string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null
            ) where T : struct, Enum
        {
            if (!Enum.TryParse<T>(param, true, out var result))
                throw new FormatException(errroMessage ?? $"无法将 \"{param}\" 解析为 {typeof(T).Name} 枚举值");

            return result;
        }

        [DebuggerStepThrough]
        public int ToInt32(string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (!int.TryParse(param, out var result))
                throw new FormatException(errroMessage ?? $"无法将 \"{param}\" 解析为 Int32");

            return result;
        }

        [DebuggerStepThrough]
        public double ToDouble(string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (!double.TryParse(param, out var result))
                throw new FormatException(errroMessage ?? $"无法将 \"{param}\" 解析为 Double");

            return result;
        }

        [DebuggerStepThrough]
        public IPAddress ToIPAddress(string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (!IPAddress.TryParse(param, out var result))
                throw new FormatException(errroMessage ?? $"无法将 \"{param}\" 解析为 IPAddress");

            return result;
        }

        [DebuggerStepThrough]
        public TimeSpan ToTimeSpan(string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (!TimeSpan.TryParse(param, out var result))
                throw new FormatException(errroMessage ?? $"无法将 \"{param}\" 解析为 TimeSpan");

            return result;
        }
    }

    // T? : notnull
    extension<T>(T? param) where T : notnull
    {
        /// <summary>
        /// 不可为空
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        [DebuggerStepThrough]
        public T Required(
            string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (param is null)
                throw new ArgumentNullException(paramName, errroMessage ?? "不可为空");

            return param;
        }
    }

    // T : notnull
    extension<T>(T param) where T : notnull, IEquatable<T>
    {
        /// <summary>
        /// 必须在指定的集合中
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [DebuggerStepThrough]
        public T In(
            ReadOnlySpan<T> values,
            string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (!values.Contains(param))
                throw new ArgumentOutOfRangeException(paramName, param, errroMessage ?? $"不在范围内");

            return param;
        }
    }


    // T : struct, IEquatable<T>
    extension<T>(T param) where T : struct, IEquatable<T>
    {
        /// <summary>
        /// 不可为默认值
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        [DebuggerStepThrough]
        public T NotDefault(
            string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (param.Equals(default))
                throw new ArgumentNullException(paramName, errroMessage ?? "不可为默认值");

            return param;
        }
    }

    // TimeSpan
    extension(TimeSpan param)
    {
        /// <summary>
        /// 必须 > 0s
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [DebuggerStepThrough]
        public TimeSpan Positive(
            string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (param <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(paramName, param, errroMessage ?? "必须大于0s");

            return param;
        }
    }

    // int
    extension(int param)
    {
        /// <summary>
        /// 不是负数
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [DebuggerStepThrough]
        public int NonNegative(
            string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (param <= 0)
                throw new ArgumentOutOfRangeException(paramName, param, errroMessage ?? "不可为负数");

            return param;
        }

        /// <summary>
        /// 不是负数
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [DebuggerStepThrough]
        public int InRange(
            int start,
            int stop,
            string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (start > stop)
                throw new ArgumentException($"start({start}) 不能大于 stop({stop})", nameof(start));

            if (param < start || param > stop)
                throw new ArgumentOutOfRangeException(paramName, param, errroMessage ?? $"超出范围. 只能在[{start}~{stop}] 之间");

            return param;
        }
    }

    // double
    extension(double param)
    {
        /// <summary>
        /// 至少达到目标值
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [DebuggerStepThrough]
        public double AtLeast(double target,
            string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (param < target)
                throw new ArgumentOutOfRangeException(paramName, param, errroMessage ?? $"至少达到目标值: {target}");

            return param;
        }
    }

    // Enum
    extension<T>(T param) where T : struct, Enum
    {
        /// <summary>
        /// 必须是已定义的枚举
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [DebuggerStepThrough]
        public T Defined(
            string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (!Enum.IsDefined(param))
                throw new ArgumentOutOfRangeException(paramName, param, errroMessage ?? $"必须在定义的枚举中");

            return param;
        }
    }
}
