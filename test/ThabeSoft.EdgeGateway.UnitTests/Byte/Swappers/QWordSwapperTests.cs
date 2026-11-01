namespace ThabeSoft.EdgeGateway.Byte.Swappers;


[TestClass]
public class QWordSwapperTests
{
    [TestMethod]
    public void Swap_BigEndian()
    {
        byte[] input = [1, 2, 3, 4, 5, 6, 7, 8];
        byte[] expected = [1, 2, 3, 4, 5, 6, 7, 8];

        QWordSwapper.BigEndian.Swap(input);

        CollectionAssert.AreEqual(expected, input);
    }

    [TestMethod]
    public void Swap_LittleEndian()
    {
        byte[] input = [1, 2, 3, 4, 5, 6, 7, 8];
        byte[] expected = [8, 7, 6, 5, 4, 3, 2, 1];

        QWordSwapper.LittleEndian.Swap(input);

        CollectionAssert.AreEqual(expected, input);
    }


    [TestMethod]
    public void Swap_ByteOnly_SwapsBytesWithinWords()
    {
        byte[] input = [1, 2, 3, 4, 5, 6, 7, 8];
        byte[] expected = [2, 1, 4, 3, 6, 5, 8, 7];   // 每 2 字节内部交换

        QWordSwapper.Create([QWordSwap.Byte]).Swap(input);
        CollectionAssert.AreEqual(expected, input);
    }

    [TestMethod]
    public void Swap_WordOnly_SwapsWordsWithinDWords()
    {
        byte[] input = [1, 2, 3, 4, 5, 6, 7, 8];
        byte[] expected = [3, 4, 1, 2, 7, 8, 5, 6];   // 每 4 字节内 Word 交换

        QWordSwapper.Create([QWordSwap.Word]).Swap(input);
        CollectionAssert.AreEqual(expected, input);
    }

    [TestMethod]
    public void Swap_DWordOnly_SwapsDWords()
    {
        byte[] input = [1, 2, 3, 4, 5, 6, 7, 8];
        byte[] expected = [5, 6, 7, 8, 1, 2, 3, 4];   // 前后 4 字节交换

        QWordSwapper.Create([QWordSwap.DWord]).Swap(input);
        CollectionAssert.AreEqual(expected, input);
    }

    [TestMethod]
    public void Swap_ByteAndWord_OrderIndependent()
    {
        byte[] input = [1, 2, 3, 4, 5, 6, 7, 8];
        byte[] expected = [1, 2, 3, 4, 5, 6, 7, 8];

        QWordSwapper.Create([QWordSwap.Byte, QWordSwap.Word]).Swap(input);
        QWordSwapper.Create([QWordSwap.Word, QWordSwap.Byte]).Swap(expected);

        CollectionAssert.AreEqual(expected, input);
    }

    [TestMethod]
    public void Swap_WordAndDWord_Produces_GHEFCDAB()
    {
        byte[] input = [1, 2, 3, 4, 5, 6, 7, 8];
        byte[] expected = [7, 8, 5, 6, 3, 4, 1, 2];

        QWordSwapper.Create([QWordSwap.Word, QWordSwap.DWord]).Swap(input);
        CollectionAssert.AreEqual(expected, input);
    }

    [TestMethod]
    public void Swap_AllThree_Equals_LittleEndian()
    {
        byte[] input = [1, 2, 3, 4, 5, 6, 7, 8];
        byte[] expected = [1, 2, 3, 4, 5, 6, 7, 8];

        QWordSwapper.Create([QWordSwap.DWord, QWordSwap.Word, QWordSwap.Byte]).Swap(input);
        QWordSwapper.LittleEndian.Swap(expected);

        CollectionAssert.AreEqual(expected, input);
    }


    [TestMethod]
    public void Swap_WrongLength_Throws()
    {
        Assert.Throws<ArgumentException>(() => DWordSwapper.ABCD.Swap([1, 2, 3]));
        Assert.Throws<ArgumentException>(() => DWordSwapper.ABCD.Swap([1, 2, 3, 4, 5, 6, 7, 8, 9]));
    }
}
