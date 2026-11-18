using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.UnitTests.Modbus;


[TestClass]
public class ModbusTagTests
{
    [TestMethod]
    public async Task Create_BitsLength()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
                    ITagDescriptor.Create(ModbusDataAddress.Coil(1, 0, 1), CodecBuilder.Byte)
                );

        TestContext.WriteLine(ex.Message);
    }

    [TestMethod]
    public async Task Create()
    {
        ITagDescriptor.Create(ModbusDataAddress.Coil(1, 0, 8), CodecBuilder.Byte);
    }


    public TestContext TestContext { get; set; }
}
