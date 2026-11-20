namespace ThabeSoft.Codecs;


internal interface ISwapper
{
    void Swap(Span<byte> bytes);
}