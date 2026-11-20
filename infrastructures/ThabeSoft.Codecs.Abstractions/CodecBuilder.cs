using ThabeSoft.Codecs.Bits;
using ThabeSoft.Codecs.Bytes;
using ThabeSoft.Codecs.DWords;
using ThabeSoft.Codecs.QWords;
using ThabeSoft.Codecs.Words;

namespace ThabeSoft.Codecs;


public static class CodecBuilder
{
    public static BitsCodecDescriptor BitsLsbFirst(int bitCount) => Bits(bitCount, BitOrder.LsbFirst);
    public static BitsCodecDescriptor BitsMsbFirst(int bitCount) => Bits(bitCount, BitOrder.MsbFirst);
    public static BitsCodecDescriptor Bits(int bitCount, BitOrder order = BitOrder.LsbFirst) => new() { RequiredBits = bitCount, BitOrder = order };


    public static ByteCodecDescriptor Byte => ByteCodecDescriptor.Instance;
    public static BoolCodecDescriptor Bool(int bitOffset) => new() { BitOffset = bitOffset };


    #region --Word--

    public static UInt16CodecDescriptor UInt16BigEndian => UInt16CodecDescriptor.BigEndian;
    public static UInt16CodecDescriptor UInt16LittleEndian => UInt16CodecDescriptor.LittleEndian;
    public static UInt16CodecDescriptor UInt16(WordEndianess endianess) => UInt16CodecDescriptor.Create(endianess);


    public static Int16CodecDescriptor Int16BigEndian => Int16CodecDescriptor.BigEndian;
    public static Int16CodecDescriptor Int16LittleEndian => Int16CodecDescriptor.LittleEndian;
    public static Int16CodecDescriptor Int16(WordEndianess endianess) => Int16CodecDescriptor.Create(endianess);


    public static CharCodecDescriptor CharBigEndian => CharCodecDescriptor.BigEndian;
    public static CharCodecDescriptor CharLittleEndian => CharCodecDescriptor.LittleEndian;
    public static CharCodecDescriptor Char(WordEndianess endianess) => CharCodecDescriptor.Create(endianess);

    #endregion


    #region --DWord--

    public static UInt32CodecDescriptor UInt32Abcd => UInt32CodecDescriptor.Abcd;
    public static UInt32CodecDescriptor UInt32Badc => UInt32CodecDescriptor.Badc;
    public static UInt32CodecDescriptor UInt32Cdab => UInt32CodecDescriptor.Cdab;
    public static UInt32CodecDescriptor UInt32Dcba => UInt32CodecDescriptor.Dcba;
    public static UInt32CodecDescriptor UInt32(DWordLayout layout) => UInt32CodecDescriptor.Create(layout);


    public static Int32CodecDescriptor Int32Abcd => Int32CodecDescriptor.Abcd;
    public static Int32CodecDescriptor Int32Badc => Int32CodecDescriptor.Badc;
    public static Int32CodecDescriptor Int32Cdab => Int32CodecDescriptor.Cdab;
    public static Int32CodecDescriptor Int32Dcba => Int32CodecDescriptor.Dcba;
    public static Int32CodecDescriptor Int32(DWordLayout layout) => Int32CodecDescriptor.Create(layout);


    public static FloatCodecDescriptor FloatAbcd => FloatCodecDescriptor.Abcd;
    public static FloatCodecDescriptor FloatBadc => FloatCodecDescriptor.Badc;
    public static FloatCodecDescriptor FloatCdab => FloatCodecDescriptor.Cdab;
    public static FloatCodecDescriptor FloatDcba => FloatCodecDescriptor.Dcba;
    public static FloatCodecDescriptor Float(DWordLayout layout) => FloatCodecDescriptor.Create(layout);


    #endregion

    #region --QWord--

    public static UInt64CodecDescriptor UInt64BigEndian => UInt64CodecDescriptor.BigEndian;
    public static UInt64CodecDescriptor UInt64LittleEndian => UInt64CodecDescriptor.LittleEndian;
    public static UInt64CodecDescriptor UInt64(params IEnumerable<QWordSwap> swaps) => new() { Swaps = [.. swaps] };


    public static Int64CodecDescriptor Int64BigEndian => Int64CodecDescriptor.BigEndian;
    public static Int64CodecDescriptor Int64LittleEndian => Int64CodecDescriptor.LittleEndian;
    public static Int64CodecDescriptor Int64(params IEnumerable<QWordSwap> swaps) => new() { Swaps = [.. swaps] };


    public static DoubleCodecDescriptor DoubleBigEndian => DoubleCodecDescriptor.BigEndian;
    public static DoubleCodecDescriptor DoubleLittleEndian => DoubleCodecDescriptor.LittleEndian;
    public static DoubleCodecDescriptor Double(params IEnumerable<QWordSwap> swaps) => new() { Swaps = [.. swaps] };


    #endregion
}
