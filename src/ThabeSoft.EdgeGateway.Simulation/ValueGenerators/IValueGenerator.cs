namespace ThabeSoft.EdgeGateway.ValueGenerators;


public interface IValueGenerator<T>
    where T : notnull
{
    void Reset();
    T Next();
}