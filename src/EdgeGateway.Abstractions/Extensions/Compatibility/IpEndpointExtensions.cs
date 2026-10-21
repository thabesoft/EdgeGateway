#if NETSTANDARD2_0

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System.Net;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

internal static class IPEndPointExtensions
{
    extension(IPEndPoint)
    {
        public static IPEndPoint Parse(ReadOnlySpan<char> iPEndPoint)
        {
            if(iPEndPoint.Length == 0)
                throw new ArgumentNullException(nameof(iPEndPoint));

            var colonIndex = iPEndPoint.IndexOf(':');
            if (colonIndex == -1)
                throw new ArgumentException("Invalid IPEndPoint format.", nameof(iPEndPoint));

            var address = iPEndPoint.Slice(0, colonIndex);
            var port = iPEndPoint[(colonIndex + 1)..];

            return new IPEndPoint(IPAddress.Parse(address.ToString()), int.Parse(port.ToString()));
        }
    }
}

#endif