#if NETSTANDARD2_0

using System.Diagnostics.CodeAnalysis;

namespace System.Net;

public static class Extensions
{
    extension(IPEndPoint)
    {
        public static bool TryParse(string? value, [NotNullWhen(true)] out IPEndPoint? result)
        {
            result = default;

            if (string.IsNullOrWhiteSpace(value)) return false;

            var items = value!.Split(':');
            if (items.Length != 2) return false;

            if (!IPAddress.TryParse(items[0], out var address)) return false;
            if (!int.TryParse(items[1], out var port)) return false;

            result = new(address, port);
            return true;
        }
    }
}

#endif