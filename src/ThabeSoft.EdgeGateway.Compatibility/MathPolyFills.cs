namespace System;

public static class MathPolyFills
{
    extension(Math)
    {
        public static double Clamp(double value, double min, double max) =>
            value < min ? min : value > max ? max : value;
    }
}
