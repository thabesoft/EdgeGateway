namespace ThabeSoft.EdgeGateway.Codecs;

internal interface ISwapper
{
    void Swap(Span<byte> bytes);
}