using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;

namespace ThabeSoft.Primitives.Guard;


public static class ThabeSoftEnumGuard
{
    extension(string param)
    {
        [DebuggerStepThrough]
        public IPAddress ToIpAddress(string? errorMessage = null,
            [CallerArgumentExpression(nameof(param))] string? paramName = null)
        {
            if (!IPAddress.TryParse(param, out var result))
                throw new FormatException(errorMessage ?? $"无法将(\"{param}\")转换为({typeof(IPAddress)}) > {paramName}");

            return result;
        }
    }
}
