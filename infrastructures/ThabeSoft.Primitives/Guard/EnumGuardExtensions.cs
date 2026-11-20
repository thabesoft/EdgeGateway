using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ThabeSoft.EdgeGateway.Primitives.Guard;


public static class EnumGuardExtensions
{
    extension<T>(T param) where T : struct, Enum
    {
        /// <summary>
        /// 必须是已定义的枚举
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [DebuggerStepThrough]
        public T Defined(
            string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
#if NETSTANDARD2_0
            if (!Enum.IsDefined(typeof(T), param))
                throw new ArgumentOutOfRangeException(paramName, param, errorMessage ?? $"必须在定义的枚举中");
#else
            if (!Enum.IsDefined(param))
                throw new ArgumentOutOfRangeException(paramName, param, errorMessage ?? $"必须在定义的枚举中");
#endif

            return param;
        }
    }


    extension(string param)
    {
        [DebuggerStepThrough]
        public T ToEnum<T>(string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null
            ) where T : struct, Enum
        {
            if (!Enum.TryParse<T>(param, true, out var result))
                throw new FormatException(errorMessage ?? $"无法将(\"{param}\")转换为({typeof(T)}) > {paramName}");

            return result;
        }
    }
}
