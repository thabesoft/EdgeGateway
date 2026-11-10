namespace ThabeSoft.EdgeGateway.Swappers;

internal interface ISwapper
{
    void Swap(Span<byte> bytes);
}