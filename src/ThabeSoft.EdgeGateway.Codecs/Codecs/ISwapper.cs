namespace ThabeSoft.EdgeGateway.Codecs;


internal interface ISwapper : IRequiredBits
{
    void Swap(Span<byte> bytes);
}