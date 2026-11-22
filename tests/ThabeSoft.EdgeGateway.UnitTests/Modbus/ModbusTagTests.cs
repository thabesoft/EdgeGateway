using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.Codecs;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.UnitTests.Modbus;


[TestClass]
public class ModbusTagTests
{
    [TestMethod]
    public async Task Create_BitsLength()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
                    TagDescriptor.Create(ModbusDataAddress.Coil(1, 0, 1), ThabeSoft.Codecs.Codec.Byte)
                );

        TestContext.WriteLine(ex.Message);
    }

    [TestMethod]
    public async Task Create()
    {
        TagDescriptor.Create(ModbusDataAddress.Coil(1, 0, 8), ThabeSoft.Codecs.Codec.Byte);
    }


    public TestContext TestContext { get; set; }
}
