using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ThabeSoft.EdgeGateway.Primitives.Guard;


public static class ObjectGuardExtensions
{
    // T
    extension(object param)
    {
        /// <summary>
        /// 必须是指定类型
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
#if !NETSTANDARD2_0
        [StackTraceHidden]
#endif
        public TResult AssertType<TResult>(string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (param is TResult result)
                return result;

            var actualType = param?.GetType()?.Name ?? "null";
            var expectType = typeof(TResult).Name;
            var msg = errorMessage ?? $"参数({paramName})不符合预期，需要 {expectType}，实际 {actualType}";

            throw new InvalidOperationException(msg);
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
            string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (param is null)
                throw new ArgumentNullException(paramName, errorMessage ?? "不可为空");

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
            string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (param.Equals(default))
                throw new ArgumentNullException(paramName, errorMessage ?? "不可为默认值");

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
            string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (!values.Contains(param))
                throw new ArgumentOutOfRangeException(paramName, param, errorMessage ?? $"不在范围内");

            return param;
        }
    }
}