using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Tag;


[TestClass]
public class ModbusTagTests
{
    [TestMethod]
    public async Task Create_BitsLength()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
                    TagDescriptor.Create(ModbusDataAddress.Coil(1, 0, 1), ByteCodec.Instance)
                );

        TestContext.WriteLine(ex.Message);
    }

    [TestMethod]
    public async Task Create()
    {
        TagDescriptor.Create(ModbusDataAddress.Coil(1, 0, 8), ByteCodec.Instance);
    }


    public TestContext TestContext { get; set; }
}
