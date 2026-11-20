using ThabeSoft.EdgeGateway.Codecs.DWords;
using ThabeSoft.EdgeGateway.Codecs.QWords;
using ThabeSoft.EdgeGateway.Codecs.Words;

namespace ThabeSoft.EdgeGateway.Codecs.Extensions;


internal static class CodecExtensions
{
    extension<TWordCodec>(TWordCodec codec)
        where TWordCodec : ICodec<ushort>
    {
        public ICodec<short> AsShort() => new ShortCodec(codec);
        public ICodec<char> AsChar() => new CharCodec(codec);
    }

    extension<TDWordCodec>(TDWordCodec codec)
        where TDWordCodec : ICodec<uint>
    {
        public ICodec<int> AsInt() => new IntCodec(codec);
        public ICodec<float> AsFloat() => new FloatCodec(codec);
    }

    extension<TQWordCodec>(TQWordCodec codec)
        where TQWordCodec : ICodec<ulong>
    {
        public ICodec<long> AsLong() => new LongCodec(codec);
        public ICodec<double> AsDouble() => new DoubleCodec(codec);
    }
}