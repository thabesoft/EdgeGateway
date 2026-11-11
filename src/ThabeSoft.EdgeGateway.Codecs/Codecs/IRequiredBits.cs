namespace ThabeSoft.EdgeGateway.Codecs;

public interface IRequiredBits
{
    int RequiredBits { get; }
}

public static class RequiredBitsExtensions
{
    extension(IRequiredBits requiredBits)
    {
        public int RequiredBytes
        {
            get
            {
                if (requiredBits.RequiredBits <= 0) return 0;
                return (requiredBits.RequiredBits + 7) / 8;
            }
        }

        public int RequiredWords
        {
            get
            {
                if (requiredBits.RequiredBits <= 0) return 0;
                return (requiredBits.RequiredBits + 15) / 16;
            }
        }
    }
}