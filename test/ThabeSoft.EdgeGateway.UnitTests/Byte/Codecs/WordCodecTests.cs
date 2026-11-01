namespace ThabeSoft.EdgeGateway.Byte.Codecs;


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

        Span<byte> cache = stackalloc byte[WordCodec.ByteLength];
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

        Span<byte> cache = stackalloc byte[WordCodec.ByteLength];
        codec.Encode(value, cache);
        var expected = codec.Decode(cache);

        Assert.AreEqual(expected, value);
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
