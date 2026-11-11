namespace ThabeSoft.EdgeGateway.Codecs.Internal;

internal interface ISwapper
{
    void Swap(Span<byte> bytes);
}