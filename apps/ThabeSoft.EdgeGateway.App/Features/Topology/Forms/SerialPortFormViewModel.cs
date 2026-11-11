using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;

internal sealed partial class SerialPortFormViewModel : ViewModelBase
{
    public SerialParity[] ParitysOptions { get; } = [SerialParity.None, SerialParity.Odd, SerialParity.Even, SerialParity.Mark, SerialParity.Space];
    public SerialStopBits[] StopBitsOptions { get; } = [SerialStopBits.One, SerialStopBits.OnePointFive, SerialStopBits.Two];
    public int[] BaudRatesOptions { get; } = SerialTransportDescriptor.StandardBaudRates;
    public string[] PortNameOptions => SerialPort.GetPortNames();


    public SerialPortFormViewModel()
    {
        if (PortNameOptions.Length == 1)
            PortName = PortNameOptions[0];
    }



    [ObservableProperty]
    public partial string PortName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int BaudRate { get; set; } = 9600;

    [ObservableProperty]
    public partial SerialParity Parity { get; set; } = SerialParity.None;

    [ObservableProperty]
    public partial int DataBits { get; set; } = 8;

    [ObservableProperty]
    public partial SerialStopBits StopBits { get; set; } = SerialStopBits.One;


    public static FuncValueConverter<int, string> BaudRateStringConverter { get; } = new
    (
        value => value.ToString(),
        value => int.TryParse(value, out var result) ? result : 0
    );



    partial void OnPortNameChanged(string value)
    {
        SerialTransportDescriptor.ValidatePortName(value, out var errorMessage);
        TrySetPropertyError(nameof(PortName), errorMessage);
    }
    partial void OnBaudRateChanged(int value)
    {
        SerialTransportDescriptor.ValidateBaudRate(value, out var errorMessage);
        TrySetPropertyError(nameof(BaudRate), errorMessage);
    }
    partial void OnParityChanged(SerialParity value)
    {
        SerialTransportDescriptor.ValidateParity(value, out var errorMessage);
        TrySetPropertyError(nameof(Parity), errorMessage);
    }
    partial void OnDataBitsChanged(int value)
    {
        SerialTransportDescriptor.ValidateDataBits(value, out var errorMessage);
        TrySetPropertyError(nameof(DataBits), errorMessage);
    }
    partial void OnStopBitsChanged(SerialStopBits value)
    {
        SerialTransportDescriptor.ValidateStopBits(value, out var errorMessage);
        TrySetPropertyError(nameof(StopBits), errorMessage);
    }


    public SerialTransportDescriptor Build() =>
        SerialTransportDescriptor.Create(PortName, BaudRate, Parity, DataBits, StopBits);

    public void ValidateAllProperty()
    {
        OnPortNameChanged(PortName);
        OnBaudRateChanged(BaudRate);
        OnParityChanged(Parity);
        OnDataBitsChanged(DataBits);
        OnStopBitsChanged(StopBits);
    }
}