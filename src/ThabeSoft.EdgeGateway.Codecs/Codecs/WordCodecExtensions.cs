namespace ThabeSoft.EdgeGateway.Codecs;

public static class WordCodecExtensions
{
    extension(WordCodec codec)
    {
        public ShortCodec AsShort() => new(codec);
        public CharCodec AsChar() => new(codec);
    }
}