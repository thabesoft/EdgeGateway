using System.Buffers;
using System.Buffers.Binary;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Codecs.DWords;
using ThabeSoft.EdgeGateway.Codecs.Qword;
using ThabeSoft.EdgeGateway.Codecs.Words;

namespace ThabeSoft.EdgeGateway;


public static class CodecExtensions
{
    extension<TWordCodec>(TWordCodec codec)
        where TWordCodec : ICodec<ushort>, IRequiredBits
    {
        public ICodec<short> AsShort() => new ShortCodec(codec);
        public ICodec<char> AsChar() => new CharCodec(codec);
    }

    extension<TDWordCodec>(TDWordCodec codec)
        where TDWordCodec : ICodec<uint>, IRequiredBits
    {
        public ICodec<int> AsInt() => new IntCodec(codec);
        public ICodec<float> AsFloat() => new FloatCodec(codec);
    }

    extension<TQWordCodec>(TQWordCodec codec)
        where TQWordCodec : ICodec<ulong>, IRequiredBits
    {
        public ICodec<long> AsLong() => new LongCodec(codec);
        public ICodec<double> AsDouble() => new DoubleCodec(codec);
    }



    extension<T>(ICodec<T> codec) where T : notnull
    {
        public T DecodeBits(ReadOnlySpan<bool> bits)
        {
            ArgumentNullException.ThrowIfNull(codec);
            if (bits.Length == 0) throw new ArgumentException("数据不可为空", nameof(bits));

            var byteLength = (bits.Length + 7) / 8;
            Span<byte> cache = stackalloc byte[byteLength];

            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i])
                    cache[i / 8] |= (byte)(1 << (i % 8));
            }

            return codec.Decode(cache);
        }

        public T DecodeWords(ReadOnlySpan<ushort> words)
        {
            ArgumentNullException.ThrowIfNull(codec);
            if (words.Length == 0) throw new ArgumentException("数据不可为空", nameof(words));

            Span<byte> cache = stackalloc byte[words.Length * 2];

            for (int i = 0; i < words.Length; i++)
                BinaryPrimitives.WriteUInt16BigEndian(cache[(i * 2)..], words[i]);

            return codec.Decode(cache);
        }


        public int EncodeWords(T value, Span<ushort> words, bool bigEndian = true)
        {
            ArgumentNullException.ThrowIfNull(codec);
            if (codec is not IRequiredBits requiredBits)
                throw new InvalidOperationException("无法确认编码器所需数据长度");

            var byteCount = requiredBits.RequiredBytes;
            if (byteCount % 2 != 0)
                throw new InvalidOperationException("编码器字节数必须是偶数");

            var wordCount = byteCount / 2;
            if (words.Length < wordCount)
                throw new ArgumentException($"目标缓冲区至少需要 {wordCount} 个 word", nameof(words));


            var pool = ArrayPool<byte>.Shared;
            var cache = pool.Rent(byteCount);
            var span = cache.AsSpan(0, byteCount);
            try
            {
                var length = codec.Encode(value, span);
                if (length != byteCount)
                    throw new InvalidOperationException($"编码器返回 {length} 字节，预期 {byteCount}");

                for (int i = 0; i < wordCount; i++)
                {
                    var slice = cache.AsSpan(i * 2, 2);
                    words[i] = bigEndian
                        ? BinaryPrimitives.ReadUInt16BigEndian(slice)
                        : BinaryPrimitives.ReadUInt16LittleEndian(slice);
                }

                return wordCount;
            }
            finally
            {
                pool.Return(cache);
            }
        }

        public ushort[] EncodeWords(T value, bool bigEndian = true)
        {
            if (codec is not IRequiredBits requiredBits)
                throw new InvalidOperationException("无法确认编码器所需数据长度");

            var wordCount = requiredBits.RequiredWords;
            var words = new ushort[wordCount];
            codec.EncodeWords(value, words, bigEndian);
            return words;
        }
    }
}
