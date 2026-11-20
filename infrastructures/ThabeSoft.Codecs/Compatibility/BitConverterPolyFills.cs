
#if NETSTANDARD2_0

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class BitConverterPolyFills
{
    extension(BitConverter)
    {
        public static uint SingleToUInt32Bits(float value)
        {
            unsafe { return *(uint*)&value; }
        }
        public static float UInt32BitsToSingle(uint value)
        {
            unsafe { return *(float*)&value; }
        }

        public static ulong DoubleToUInt64Bits(double value)
        {
            unsafe { return *(ulong*)&value; }
        }
        public static double UInt64BitsToDouble(ulong value)
        {
            unsafe { return *(double*)&value; }
        }
    }
}

#endif