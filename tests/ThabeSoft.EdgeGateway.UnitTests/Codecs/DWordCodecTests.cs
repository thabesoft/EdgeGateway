using ThabeSoft.EdgeGateway.Codecs.DWords;

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

        Span<byte> cache = stackalloc byte[DWordCodec.RequiredBytes];
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

        Span<byte> cache = stackalloc byte[DWordCodec.RequiredBytes];
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

        Span<byte> cache = stackalloc byte[DWordCodec.RequiredBytes];
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

        Span<byte> cache = stackalloc byte[DWordCodec.RequiredBytes];
        codec.Encode(value, cache);
        var expected = codec.Decode(cache);

        Assert.AreEqual(expected, value);
    }



    [TestMethod]
    [DataRow(3.14f)]
    [DataRow(0.001f)]
    [DataRow(123456f)]
    [DataRow(-123f)]
    [DataRow(-12.3f)]
    public void BigEndian_AsFloat_RoundTrip(float expected)
    {
        var codec = DWordCodec.BigEndian.AsFloat();
        Span<byte> buffer = stackalloc byte[codec.RequiredBytes];

        codec.Encode(expected, buffer);
        var actual = codec.Decode(buffer);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow(12345)]
    public void BigEndian_AsShort_RoundTrip(int expected)
    {
        var codec = DWordCodec.BigEndian.AsInt();
        Span<byte> buffer = stackalloc byte[codec.RequiredBytes];

        codec.Encode(expected, buffer);
        var actual = codec.Decode(buffer);

        Assert.AreEqual(expected, actual);
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
