namespace EdgeGateway.Data;


/// <summary>
/// 数据描述符，包含数据的地址和类型描述符
/// </summary>
/// <typeparam name="T">数据在C#中的类型</typeparam>
public record class DataDescriptor<T>(IDataAddress Address, ITypeDescriptor<T> TypeDescriptor)
{
    /// <summary>
    /// 返回数据描述符的字符串表示。
    /// </summary>
    /// <returns>
    /// 格式为 <c>Address(TypeName)</c>，例如：
    /// <list type="bullet">
    /// <item><description><c>0x01.HR1(Int32)</c></description></item>
    /// <item><description><c>0x01.HR3[2](Float)</c></description></item>
    /// <item><description><c>0x01.C11(Boolean)</c></description></item>
    /// </list>
    /// </returns>
    public override string ToString()
    {
        return $"{Address}({typeof(T).Name})";
    }
}

public static class DataDescriptor
{
    public static DataDescriptor<T> Create<T>(IDataAddress address, ITypeDescriptor<T> typeDescriptor)
    {
        return new DataDescriptor<T>(address, typeDescriptor);
    }
}