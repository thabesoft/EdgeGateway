using FluentResults;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EdgeGateway.Data;


/// <summary>
/// 固定长度类型描述器，适用于所有固定长度的非托管类型（如 int、float、double 等）
/// </summary>
public sealed class UnmanagedTypeDescriptor<T> : ITypeDescriptor<T>
    where T : unmanaged
{
    private UnmanagedTypeDescriptor() { }
    public static UnmanagedTypeDescriptor<T> Instance { get; } = new();


    public string Name { get; } = typeof(T).Name;
    public int ByteLength { get; } = Unsafe.SizeOf<T>();
    int? ITypeDescriptor<T>.ByteLength => ByteLength;

    public Result<T> Decode(ReadOnlySpan<byte> rawData)
    {
        if (rawData.Length < ByteLength)
            return Result.Fail<T>("Insufficient data length for decoding Word.");

        return MemoryMarshal.Cast<byte, T>(rawData)[0];
    }
    public Result<int> Encode(T value, Span<byte> buffer)
    {
        if (buffer.Length < ByteLength)
            return Result.Fail<int>("Insufficient buffer length for encoding Word.");

        Span<T> intSpan = [value];
        Span<byte> byteSpan = MemoryMarshal.Cast<T, byte>(intSpan);

        byteSpan.CopyTo(buffer[..ByteLength]);

        return Result.Ok(byteSpan.Length);
    }
}