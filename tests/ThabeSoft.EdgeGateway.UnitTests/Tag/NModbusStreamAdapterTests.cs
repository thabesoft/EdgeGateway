using Moq;
using ThabeSoft.EdgeGateway.IO;

namespace ThabeSoft.EdgeGateway.Tag;


[TestClass]
public class NModbusStreamAdapterTests
{
    [TestMethod]
    public void Read_UsesOffsetAndCount()
    {
        var mockChannel = new Mock<IByteStream>();
        mockChannel
            .Setup(x => x.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()))
            .Returns((Memory<byte> buffer, CancellationToken _) =>
            {
                buffer.Span[0] = 0xAA;
                return ValueTask.FromResult(1);
            });

        var adapter = new NModbusStreamAdapter(mockChannel.Object);
        byte[] buffer = new byte[10];

        int read = adapter.Read(buffer, offset: 3, count: 4);

        Assert.AreEqual(1, read);
        Assert.AreEqual(0xAA, buffer[3]);          // 写到了 offset 位置
        Assert.AreEqual(0, buffer[2]);             // offset 之前没动
        Assert.AreEqual(0, buffer[4]);             // 只写了 1 字节，count 内其余没动
        mockChannel.Verify(
            x => x.ReadAsync(It.Is<Memory<byte>>(m => m.Length == 4), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public void Read_Timeout_ThrowsTimeoutException()
    {
        var mockChannel = new Mock<IByteStream>();
        mockChannel
            .Setup(x => x.ReadAsync(It.IsAny<Memory<byte>>(), It.IsAny<CancellationToken>()))
            .Returns(async (Memory<byte> _, CancellationToken ct) =>
            {
                await Task.Delay(Timeout.Infinite, ct);
                return 0;
            });

        var adapter = new NModbusStreamAdapter(mockChannel.Object) { ReadTimeout = 50 };

        Assert.Throws<TimeoutException>(() => adapter.Read(new byte[1], 0, 1));
    }


    [TestMethod]
    public void Write_UsesOffsetAndCount()
    {
        var mockChannel = new Mock<IByteStream>();
        mockChannel
            .Setup(x => x.WriteAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var adapter = new NModbusStreamAdapter(mockChannel.Object);
        byte[] buffer = [0, 1, 2, 3, 4, 5];

        adapter.Write(buffer, offset: 2, count: 3);

        mockChannel.Verify(
            x => x.WriteAsync(It.Is<ReadOnlyMemory<byte>>(m => m.Length == 3), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
