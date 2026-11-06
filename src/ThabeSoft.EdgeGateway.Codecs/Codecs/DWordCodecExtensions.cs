namespace ThabeSoft.EdgeGateway.Codecs;

public static class DWordCodecExtensions
{
    extension(DWordCodec codec)
    {
        public IntCodec AsInt() => new(codec);
        public FloatCodec AsFloat() => new(codec);
    }
}