namespace ThabeSoft.EdgeGateway.App.Services;

public interface ISerialPortNamesProvider
{
    string[] GetPortNames();
}