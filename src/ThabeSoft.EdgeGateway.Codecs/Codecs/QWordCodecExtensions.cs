namespace ThabeSoft.EdgeGateway.Codecs;

public static class QWordCodecExtensions
{
    extension(QWordCodec codec)
    {
        public LongCodec AsLong() => new(codec);
        public DoubleCodec AsDouble() => new(codec);
    }
}