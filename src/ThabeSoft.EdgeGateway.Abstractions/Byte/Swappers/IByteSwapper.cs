namespace ThabeSoft.EdgeGateway.Byte.Swappers;

public interface IByteSwapper
{
    void Swap(Span<byte> bytes);
}