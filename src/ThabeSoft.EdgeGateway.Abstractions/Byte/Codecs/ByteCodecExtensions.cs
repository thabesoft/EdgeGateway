using System.Buffers.Binary;

namespace ThabeSoft.EdgeGateway.Byte.Codecs;

public static class ByteCodecExtensions
{
    extension<T>(IByteCodec<T> codec) where T : notnull
    {
        public T DecodeBits(bool[] bits)
        {
            ArgumentNullException.ThrowIfNull(codec);
            ArgumentNullException.ThrowIfNull(bits);

            var byteLength = (bits.Length + 7) / 8;
            Span<byte> cache = stackalloc byte[byteLength];

            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i])
                    cache[i / 8] |= (byte)(1 << (i % 8));
            }

            return codec.Decode(cache);
        }

        public T DecodeWords(ushort[] words)
        {
            ArgumentNullException.ThrowIfNull(codec);
            ArgumentNullException.ThrowIfNull(words);

            Span<byte> cache = stackalloc byte[words.Length * 2];

            for (int i = 0; i < words.Length; i++)
                BinaryPrimitives.WriteUInt16BigEndian(cache[(i * 2)..], words[i]);

            return codec.Decode(cache);
        }
    }
}
