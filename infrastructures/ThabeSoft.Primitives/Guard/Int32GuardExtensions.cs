using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ThabeSoft.Primitives.Guard;


public static class Int32GuardExtensions
{
    extension(int param)
    {
        /// <summary>
        /// 必须 > 0
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [DebuggerStepThrough]
        public int Positive(
            string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null) 
        {
            if (param <= 0)
                throw new ArgumentOutOfRangeException(paramName, param, errorMessage ?? "不可为负数");

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
        public int ToInt32(string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (!int.TryParse(param, out var result))
                throw new FormatException(errorMessage ?? $"无法将(\"{param}\")转换为({typeof(int)}) > {paramName}");

            return result;
        }
    }
}
