namespace ThabeSoft.EdgeGateway.Codecs.Swappers;

public interface ISwapper
{
    void Swap(Span<byte> bytes);
}