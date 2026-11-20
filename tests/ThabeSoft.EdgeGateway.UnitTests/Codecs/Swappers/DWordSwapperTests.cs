using ThabeSoft.Codecs.DWords;

namespace ThabeSoft.EdgeGateway.UnitTests.Codecs.Swappers;


[TestClass]
public class DWordSwapperTests
{
    [TestMethod]
    public void Swap_ABCD()
    {
        byte[] input = [1, 2, 3, 4];
        byte[] result = [1, 2, 3, 4];

        DWordSwapper.Abcd.Swap(input);

        CollectionAssert.AreEqual(result, input);
    }

    [TestMethod]
    public void Swap_BADC()
    {
        byte[] input = [1, 2, 3, 4];
        byte[] result = [2, 1, 4, 3];

        DWordSwapper.Badc.Swap(input);

        CollectionAssert.AreEqual(result, input);
    }

    [TestMethod]
    public void Swap_CDAB()
    {
        byte[] input = [1, 2, 3, 4];
        byte[] result = [3, 4, 1, 2];

        DWordSwapper.Cdab.Swap(input);

        CollectionAssert.AreEqual(result, input);
    }

    [TestMethod]
    public void Swap_DCBA()
    {
        byte[] input = [1, 2, 3, 4];
        byte[] result = [4, 3, 2, 1];

        DWordSwapper.Dcba.Swap(input);

        CollectionAssert.AreEqual(result, input);
    }



    [TestMethod]
    public void Swap_WrongLength_Throws()
    {
        Assert.Throws<ArgumentException>(() => DWordSwapper.Abcd.Swap([1, 2, 3]));
        Assert.Throws<ArgumentException>(() => DWordSwapper.Abcd.Swap([1, 2, 3, 4, 5]));
    }
}
