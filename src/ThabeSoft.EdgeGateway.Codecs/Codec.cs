using System.Collections;
using ThabeSoft.EdgeGateway.Codecs.Internal;
using ThabeSoft.EdgeGateway.Codecs.Internal.Bytes;
using ThabeSoft.EdgeGateway.Codecs.Internal.DWords;
using ThabeSoft.EdgeGateway.Codecs.Internal.Qword;
using ThabeSoft.EdgeGateway.Codecs.Internal.Words;

namespace ThabeSoft.EdgeGateway.Codecs;


public static class Codec
{
    public static ICodec<byte> Byte { get; } = ByteCodec.Instance;

    public static ICodec<ushort> UInt16 { get; } = WordCodec.BigEndian;
    public static ICodec<short> Int16 { get; } = WordCodec.BigEndian.AsShort();
    public static ICodec<char> Char { get; } = WordCodec.BigEndian.AsChar();


    public static ICodec<uint> UInt32 { get; } = DWordCodec.BigEndian;
    public static ICodec<int> Int32 { get; } = DWordCodec.BigEndian.AsInt();
    public static ICodec<float> Float { get; } = DWordCodec.BigEndian.AsFloat();


    public static ICodec<ulong> UInt64 { get; } = QWordCodec.BigEndian;
    public static ICodec<long> Int64 { get; } = QWordCodec.BigEndian.AsLong();
    public static ICodec<double> Double { get; } = QWordCodec.BigEndian.AsDouble();




    public static ICodec<BitArray> BitsLsbFirst(int bitCount) =>
        new BitsCodec(bitCount, BitOrder.LsbFirst);
    public static ICodec<BitArray> BitsMsbFirst(int bitCount) =>
        new BitsCodec(bitCount, BitOrder.MsbFirst);

    public static ICodec<bool> Bool(int bitOffset) =>
        BoolCodec.BitOffset(bitOffset);
}
