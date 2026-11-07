namespace ThabeSoft.Simulator.ValueGenerators;


public interface IValueGenerator<T>
    where T : notnull
{
    void Reset();
    T Next();
}