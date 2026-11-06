
#if NETSTANDARD2_0

namespace System;

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