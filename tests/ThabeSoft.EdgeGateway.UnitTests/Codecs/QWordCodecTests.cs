namespace ThabeSoft.EdgeGateway.Codecs;


[TestClass]
public class QWordCodecTests
{
    [TestMethod]
    [DataRow((uint)0)]
    [DataRow((uint)1)]
    [DataRow((uint)0x1234)]
    [DataRow(uint.MaxValue)]
    public void BigEndian_RoundTrip(uint value)
    {
        var codec = QWordCodec.BigEndian;

        Span<byte> cache = stackalloc byte[QWordCodec.ByteLength];
        codec.Encode(value, cache);
        var expected = codec.Decode(cache);

        Assert.AreEqual(expected, value);
    }

    [TestMethod]
    [DataRow((uint)0)]
    [DataRow((uint)1)]
    [DataRow((uint)0x1234)]
    [DataRow(uint.MaxValue)]
    public void LittleEndian_RoundTrip(uint value)
    {
        var codec = QWordCodec.LittleEndian;

        Span<byte> cache = stackalloc byte[QWordCodec.ByteLength];
        codec.Encode(value, cache);
        var expected = codec.Decode(cache);

        Assert.AreEqual(expected, value);
    }


    [TestMethod]
    public void Encode_BufferSmall_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            QWordCodec.BigEndian.Encode(0, []));
    }

    [TestMethod]
    public void Encode_DataShort_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            QWordCodec.BigEndian.Decode([]));
    }
}
