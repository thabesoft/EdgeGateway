namespace EdgeGateway.Data;


public static class TypeDescriptors
{
    public static UnmanagedTypeDescriptor<T> Unmanaged<T>() where T : unmanaged
        => UnmanagedTypeDescriptor<T>.Instance;

    public static DynamicTypeDescriptor Dyanmic()
        => new DynamicTypeDescriptor();
}