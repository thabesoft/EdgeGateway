using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace ThabeSoft.EdgeGateway.Compatibility;


public static class Guard
{
    public static ImmutableArray<T> ToImmutableArrayOrEmpty<T>(this IEnumerable<T> values)
    {
        return values?.Where(x => x is not null).ToImmutableArray() ?? [];
    }



    extension<T>(T param) where T : class
    {
        /// <summary>
        /// 不可为空
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public T Required(
            string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (param is null)
                throw new ArgumentNullException(paramName, errroMessage ?? "不可为空");

            return param;
        }
    }

    extension(TimeSpan param)
    {
        /// <summary>
        /// 必须 > 0s
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public TimeSpan Positive(
            string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (param <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(paramName, param, errroMessage ?? "必须大于0s");

            return param;
        }
    }

    extension(double param)
    {
        /// <summary>
        /// 至少达到目标
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public double AtLeast(double target,
            string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (param < target)
                throw new ArgumentOutOfRangeException(paramName, param, errroMessage ?? $"至少大于{target}");

            return param;
        }
    }

    extension(byte param)
    {
        /// <summary>
        /// 至少达到目标
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public double AtLeast(byte target,
            string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (param < target)
                throw new ArgumentOutOfRangeException(paramName, param, errroMessage ?? $"至少大于{target}");

            return param;
        }
    }


    extension<T>(T param) where T : struct, Enum
    {
        /// <summary>
        /// 必须是已定义的枚举
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public T Defined(
            string? errroMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (Enum.IsDefined(param))
                throw new ArgumentOutOfRangeException(paramName, param, errroMessage ?? $"必须在定义的枚举中");

            return param;
        }
    }
}
