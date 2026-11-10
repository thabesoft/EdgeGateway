using ThabeSoft.EdgeGateway.Codecs.Qword;

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
    [DataRow(3.14d)]
    [DataRow(6.66d)]
    [DataRow(-123d)]
    [DataRow(-123.465d)]
    public void BigEndian_AsDouble_RoundTrip(double expected)
    {
        var codec = QWordCodec.BigEndian.AsDouble();
        Span<byte> buffer = stackalloc byte[QWordCodec.ByteLength];

        codec.Encode(expected, buffer);
        var actual = codec.Decode(buffer);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow(12345678987654321)]
    [DataRow(-1)]
    public void BigEndian_AsLong_RoundTrip(long expected)
    {
        var codec = QWordCodec.BigEndian.AsLong();
        Span<byte> buffer = stackalloc byte[QWordCodec.ByteLength];

        codec.Encode(expected, buffer);
        var actual = codec.Decode(buffer);

        Assert.AreEqual(expected, actual);
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
