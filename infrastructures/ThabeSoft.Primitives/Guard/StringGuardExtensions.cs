using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ThabeSoft.EdgeGateway.Primitives.Guard;


public static class StringGuardExtensions
{
    extension(string? param)
    {
        /// <summary>
        /// 不可为空或空白字符
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        [DebuggerStepThrough]
        public string NotEmpty(
            string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentException(errorMessage ?? "字符串不可为空", paramName);

            return param!;
        }

        /// <summary>
        /// 字符串不可为空或空白
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public string NotNullOrWhiteSpace(
            string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (string.IsNullOrWhiteSpace(paramName))
                throw new ArgumentException(errorMessage ?? "字符串不可为空或空白", paramName);

            return param!;
        }
    }
}
