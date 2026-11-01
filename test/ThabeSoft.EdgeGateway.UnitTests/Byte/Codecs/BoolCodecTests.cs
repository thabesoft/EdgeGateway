namespace ThabeSoft.EdgeGateway.Byte.Codecs;


[TestClass]
public class BoolCodecTests
{
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(3)]
    [DataRow(7)]
    public void Encode_True_SetsCorrectBit(int bitOffset)
    {
        var codec = BoolCodec.BitOffset(bitOffset);
        byte[] buffer = new byte[1];

        codec.Encode(true, buffer);

        Assert.AreEqual(1 << bitOffset, buffer[0]);   // 只有第 bitOffset 位为 1
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(3)]
    [DataRow(7)]
    public void Encode_False_ClearsCorrectBit(int bitOffset)
    {
        var codec = BoolCodec.BitOffset(bitOffset);
        byte[] buffer = [0xFF];   // 全 1

        codec.Encode(false, buffer);

        Assert.AreEqual((byte)~(1 << bitOffset), buffer[0]);   // 只有第 bitOffset 位被清 0
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(3)]
    [DataRow(7)]
    public void Decode_ReadsCorrectBit(int bitOffset)
    {
        var codec = BoolCodec.BitOffset(bitOffset);
        byte[] raw = [(byte)(1 << bitOffset)];   // 只有第 bitOffset 位为 1

        Assert.IsTrue(codec.Decode(raw));

        raw[0] = (byte)~(1 << bitOffset);        // 只有第 bitOffset 位为 0
        Assert.IsFalse(codec.Decode(raw));
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(8)]
    [DataRow(int.MaxValue)]
    public void BitOffset_OutOfRange_Throws(int bitOffset)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            BoolCodec.BitOffset(bitOffset));
    }

    [TestMethod]
    public void Encode_BufferTooSmall_Throws()
    {
        var codec = BoolCodec.BitOffset(0);
        byte[] buffer = [];   // 0 字节

        Assert.Throws<ArgumentException>(() => 
            codec.Encode(true, buffer));
    }

    [TestMethod]
    public void Decode_RawTooSmall_Throws()
    {
        var codec = BoolCodec.BitOffset(0);
        byte[] raw = [];   // 0 字节

        Assert.Throws<ArgumentException>(() =>
            codec.Decode(raw));
    }

    [TestMethod]
    public void BitOffset_ReturnsCachedInstance()
    {
        var a = BoolCodec.BitOffset(3);
        var b = BoolCodec.BitOffset(3);

        Assert.AreSame(a, b);   // 缓存返回同一实例
    }

    [TestMethod]
    public void EncodeDecode_RoundTrip()
    {
        for (int offset = 0; offset < 8; offset++)
        {
            var codec = BoolCodec.BitOffset(offset);
            byte[] buffer = new byte[1];

            codec.Encode(true, buffer);
            Assert.IsTrue(codec.Decode(buffer));

            codec.Encode(false, buffer);
            Assert.IsFalse(codec.Decode(buffer));
        }
    }
}
