namespace ThabeSoft.EdgeGateway.Byte.Swappers;


[TestClass]
public class DWordSwapperTests
{
    [TestMethod]
    public void Swap_ABCD()
    {
        byte[] input = [1, 2, 3, 4];
        byte[] result = [1, 2, 3, 4];

        DWordSwapper.ABCD.Swap(input);

        CollectionAssert.AreEqual(result, input);
    }

    [TestMethod]
    public void Swap_BADC()
    {
        byte[] input = [1, 2, 3, 4];
        byte[] result = [2, 1, 4, 3];

        DWordSwapper.BADC.Swap(input);

        CollectionAssert.AreEqual(result, input);
    }

    [TestMethod]
    public void Swap_CDAB()
    {
        byte[] input = [1, 2, 3, 4];
        byte[] result = [3, 4, 1, 2];

        DWordSwapper.CDAB.Swap(input);

        CollectionAssert.AreEqual(result, input);
    }

    [TestMethod]
    public void Swap_DCBA()
    {
        byte[] input = [1, 2, 3, 4];
        byte[] result = [4, 3, 2, 1];

        DWordSwapper.DCBA.Swap(input);

        CollectionAssert.AreEqual(result, input);
    }



    [TestMethod]
    public void Swap_WrongLength_Throws()
    {
        Assert.Throws<ArgumentException>(() => DWordSwapper.ABCD.Swap([1, 2, 3]));
        Assert.Throws<ArgumentException>(() => DWordSwapper.ABCD.Swap([1, 2, 3, 4, 5]));
    }
}
