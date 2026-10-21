namespace EdgeGateway.Bytes.Swappers;


[TestClass]
public class WordSwapperTests
{
    [TestMethod]
    public void Swap_LittleEndian()
    {
        byte[] input = [1, 2];
        byte[] result = [2, 1];

        WordSwapper.LittleEndian.Swap(input);

        CollectionAssert.AreEqual(result, input);
    }

    [TestMethod]
    public void Swap_BADC()
    {
        byte[] input = [1, 2];
        byte[] result = [1, 2];

        WordSwapper.BigEndian.Swap(input);

        CollectionAssert.AreEqual(result, input);
    }



    [TestMethod]
    public void Swap_WrongLength_Throws()
    {
        Assert.Throws<ArgumentException>(() => DWordSwapper.ABCD.Swap([1]));
        Assert.Throws<ArgumentException>(() => DWordSwapper.ABCD.Swap([1, 2, 3]));
    }
}
