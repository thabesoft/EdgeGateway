using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ThabeSoft.Primitives.Guard;


public static class DoubleGuardExtensions
{
    extension(double param)
    {
        /// <summary>
        /// 至少达到目标值
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [DebuggerStepThrough]
        public double AtLeast(double target,
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
        /// <summary>
        /// 解析为 double
        /// </summary>
        /// <exception cref="FormatException"></exception>
        [DebuggerStepThrough]
        public double ToDouble(string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (!double.TryParse(param, out var result))
                throw new FormatException(errorMessage ?? $"无法将(\"{param}\")转换为({typeof(double)}) > {paramName}");

            return result;
        }
    }
}
