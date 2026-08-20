using FluentResults;
using System.Data;
using System.Text;

namespace EdgeGateway.Data;


[TestClass]
public class NumberTypeDescriptorTests
{
    [TestMethod]
    [DataRow(0x00000000, 100, 10, 10, DisplayName = "零值缩放偏移")]
    [DataRow(0x00000001, 100, 10, 110, DisplayName = "最小值缩放偏移")]
    [DataRow(0x0000000A, 100, 10, 1010, DisplayName = "正数缩放偏移")]
    [DataRow(0x00000000, 1, 0, 0, DisplayName = "缩放1偏移0")]
    [DataRow(0x00000005, 2, 0, 10, DisplayName = "缩放2")]
    [DataRow(0x00000005, 1, 5, 10, DisplayName = "偏移5")]
    public void Decode_ShouldApplyScaleAndOffset(int rawValue, int scale, int offset, int expected)
    {
        Span<byte> buffer =
        [
            (byte)(rawValue & 0xFF),
            (byte)((rawValue >> 8) & 0xFF),
            (byte)((rawValue >> 16) & 0xFF),
            (byte)((rawValue >> 24) & 0xFF)
        ];

        var type = new NumberTypeDescriptor<int> { Scale = scale, Offset = offset, Unit = "℃" };

        // Act
        var result = type.Decode(buffer);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        var actual = result.Value;
        Assert.AreEqual(expected, actual, $"原始值: {rawValue:X8}, 缩放: {scale}, 偏移: {offset}, 期望: {expected}, 实际: {actual}");
    }

    [TestMethod]
    public void NOt()
    {
#if NET462
        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            _ = new NumberTypeDescriptor<Guid>()
            {
                Offset = Guid.NewGuid(),
                Scale = Guid.NewGuid()
            };
        });

        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            _ = new NumberTypeDescriptor<DateTime>()
            {
                Offset = DateTime.MinValue,
                Scale = DateTime.Now,
            };
        });

        Assert.ThrowsExactly<NotSupportedException>(() =>
        {
            _ = new NumberTypeDescriptor<TimeSpan>()
            {
                Offset = TimeSpan.MinValue,
                Scale = TimeSpan.Zero,
            };
        });
#else
        Console.WriteLine("该方法仅限于 netstandard2.0 / .NET Framework 版本");
#endif
    }
}
