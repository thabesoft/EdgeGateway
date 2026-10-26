using System.Collections;

namespace EdgeGateway.Byte.Codecs;


[TestClass]
public class BitCodecTests
{
    [TestMethod]
    public void LsbFirst_Decode()
    {
        var codec = BitCodec.LsbFirst(10);

        byte[] raw = [0b0000_0101, 0b0000_0001];
        var actual = codec.Decode(raw);

        var expected = new BitArray(
        [
            true, false, true, false, false, false, false, false,   // 第 0~7 位
            true, false,                                            // 第 8~9 位
        ]);

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void LsbFirst_Encode()
    {
        var codec = BitCodec.LsbFirst(10);

        var bits = new BitArray(
        [
            true, false, true, false, false, false, false, false,   // 第 0~7 位
            true, false,                                            // 第 8~9 位
        ]);

        Span<byte> buffer = stackalloc byte[2];   // 10 位 = 2 字节
        codec.Encode(bits, buffer);

        // LSB First：bits[0] → 字节 0 的 bit0，bits[2] → 字节 0 的 bit2
        // 字节 0 = 0b0000_0101，字节 1 = 0b0000_0001
        byte[] expected = [0b0000_0101, 0b0000_0001];

        CollectionAssert.AreEqual(expected, buffer.ToArray());
    }

    [TestMethod]
    public void LsbFirst_RoundTrip()
    {
        var codec = BitCodec.LsbFirst(10);
        var original = new BitArray(10);
        original[0] = true; original[2] = true; original[8] = true;

        byte[] buffer = new byte[codec.ByteLength];
        codec.Encode(original, buffer);
        BitArray decoded = codec.Decode(buffer);

        CollectionAssert.AreEqual(original, decoded);
    }
    [TestMethod]
    public void LsbFirst_RoundTrip_Random()
    {
        var codec = BitCodec.LsbFirst(10);
        var random = new Random(42);   // 固定种子，可复现

        for (int n = 0; n < 100; n++)
        {
            var original = new BitArray(10);
            for (int i = 0; i < 10; i++) original[i] = random.Next(2) == 1;

            byte[] buffer = new byte[codec.ByteLength];
            codec.Encode(original, buffer);
            BitArray decoded = codec.Decode(buffer);

            CollectionAssert.AreEqual(original, decoded);
        }
    }




    [TestMethod]
    public void MsbFirst_Decode()
    {
        var codec = BitCodec.MsbFirst(10);

        byte[] raw = [0b0000_0101, 0b0000_0001];
        var actual = codec.Decode(raw);

        var expected = new BitArray(
        [
            false, false, false, false, false, true, false, true,   // 字节 0：bit7~bit0
            false, false,                                           // 字节 1：bit7~bit6
        ]);

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void MsbFirst_Encode()
    {
        var codec = BitCodec.MsbFirst(10);

        var bits = new BitArray(
        [
            true, false, true, false, false, false, false, false,   // 第 0~7 位
            true, false,                                            // 第 8~9 位
        ]);

        Span<byte> buffer = stackalloc byte[2];
        codec.Encode(bits, buffer);

        // MSB First：bits[0] → 字节 0 的 bit7，bits[2] → 字节 0 的 bit5
        // 字节 0 = 0b1010_0000，字节 1 = 0b1000_0000
        byte[] expected = [0b1010_0000, 0b1000_0000];

        CollectionAssert.AreEqual(expected, buffer.ToArray());
    }

    [TestMethod]
    public void MsbFirst_RoundTrip()
    {
        var codec = BitCodec.MsbFirst(10);
        var original = new BitArray(10);
        original[0] = true; original[2] = true; original[8] = true;

        byte[] buffer = new byte[codec.ByteLength];
        codec.Encode(original, buffer);
        BitArray decoded = codec.Decode(buffer);

        CollectionAssert.AreEqual(original, decoded);
    }
    [TestMethod]
    public void MsbFirst_RoundTrip_Random()
    {
        var codec = BitCodec.MsbFirst(10);
        var random = new Random(42);   // 固定种子，可复现

        for (int n = 0; n < 100; n++)
        {
            var original = new BitArray(10);
            for (int i = 0; i < 10; i++) original[i] = random.Next(2) == 1;

            byte[] buffer = new byte[codec.ByteLength];
            codec.Encode(original, buffer);
            BitArray decoded = codec.Decode(buffer);

            CollectionAssert.AreEqual(original, decoded);
        }
    }
}
