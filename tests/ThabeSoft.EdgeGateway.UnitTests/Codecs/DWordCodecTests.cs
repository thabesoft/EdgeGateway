namespace ThabeSoft.EdgeGateway.Codecs;


[TestClass]
public class DWordCodecTests
{
    [TestMethod]
    [DataRow((uint)0)]
    [DataRow((uint)1)]
    [DataRow((uint)0x1234)]
    [DataRow(uint.MaxValue)]
    public void ABCD_RoundTrip(uint value)
    {
        var codec = DWordCodec.ABCD;

        Span<byte> cache = stackalloc byte[DWordCodec.ByteLength];
        codec.Encode(value, cache);
        var expected = codec.Decode(cache);

        Assert.AreEqual(expected, value);
    }

    [TestMethod]
    [DataRow((uint)0)]
    [DataRow((uint)1)]
    [DataRow((uint)0x1234)]
    [DataRow(uint.MaxValue)]
    public void BADC_RoundTrip(uint value)
    {
        var codec = DWordCodec.BADC;

        Span<byte> cache = stackalloc byte[DWordCodec.ByteLength];
        codec.Encode(value, cache);
        var expected = codec.Decode(cache);

        Assert.AreEqual(expected, value);
    }

    [TestMethod]
    [DataRow((uint)0)]
    [DataRow((uint)1)]
    [DataRow((uint)0x1234)]
    [DataRow(uint.MaxValue)]
    public void CDAB_RoundTrip(uint value)
    {
        var codec = DWordCodec.CDAB;

        Span<byte> cache = stackalloc byte[DWordCodec.ByteLength];
        codec.Encode(value, cache);
        var expected = codec.Decode(cache);

        Assert.AreEqual(expected, value);
    }

    [TestMethod]
    [DataRow((uint)0)]
    [DataRow((uint)1)]
    [DataRow((uint)0x1234)]
    [DataRow(uint.MaxValue)]
    public void DCBA_RoundTrip(uint value)
    {
        var codec = DWordCodec.DCBA;

        Span<byte> cache = stackalloc byte[DWordCodec.ByteLength];
        codec.Encode(value, cache);
        var expected = codec.Decode(cache);

        Assert.AreEqual(expected, value);
    }


    [TestMethod]
    public void Encode_BufferSmall_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            DWordCodec.BigEndian.Encode(0, []));
    }
    [TestMethod]
    public void Encode_DataShort_ThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            DWordCodec.BigEndian.Decode([]));
    }
}
