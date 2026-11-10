using ThabeSoft.EdgeGateway.Codecs.Words;

namespace ThabeSoft.EdgeGateway.Codecs;


[TestClass]
public class WordCodecTests
{
    [TestMethod]
    [DataRow((ushort)0)]
    [DataRow((ushort)1)]
    [DataRow((ushort)0x1234)]
    [DataRow(ushort.MaxValue)]
    public void BigEndian_RoundTrip(ushort value)
    {
        var codec = WordCodec.BigEndian;

        Span<byte> cache = stackalloc byte[WordCodec.RequiredBytes];
        codec.Encode(value, cache);
        var expected = codec.Decode(cache);

        Assert.AreEqual(expected, value);
    }

    [TestMethod]
    [DataRow((ushort)0)]
    [DataRow((ushort)1)]
    [DataRow((ushort)0x1234)]
    [DataRow(ushort.MaxValue)]
    public void LittleEndian_RoundTrip(ushort value)
    {
        var codec = WordCodec.LittleEndian;

        Span<byte> cache = stackalloc byte[WordCodec.RequiredBytes];
        codec.Encode(value, cache);
        var expected = codec.Decode(cache);

        Assert.AreEqual(expected, value);
    }


    [TestMethod]
    [DataRow('A')]
    [DataRow('♥')]
    [DataRow('0')]
    public void BigEndian_AsChar_RoundTrip(char expected)
    {
        var codec = WordCodec.BigEndian.AsChar();
        Span<byte> buffer = stackalloc byte[WordCodec.RequiredBytes];

        codec.Encode(expected, buffer);
        var actual = codec.Decode(buffer);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow((short)12345)]
    public void BigEndian_AsShort_RoundTrip(short expected)
    {
        var codec = WordCodec.BigEndian.AsShort();
        Span<byte> buffer = stackalloc byte[WordCodec.RequiredBytes];

        codec.Encode(expected, buffer);
        var actual = codec.Decode(buffer);

        Assert.AreEqual(expected, actual);
    }





    [TestMethod]
    public void Encode_BufferSmall_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            WordCodec.BigEndian.Encode(0, []));
    }
    [TestMethod]
    public void Encode_DataShort_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            WordCodec.BigEndian.Decode([]));
    }
}
