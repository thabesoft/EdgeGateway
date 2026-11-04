using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO.Ports;
using System.Net;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.App.Features.Topology;

internal sealed partial class SerialPortFormViewModel : ViewModelBase
{
    public Parity[] ParitysOptions { get; } = [Parity.None, Parity.Odd, Parity.Even, Parity.Mark, Parity.Space];
    public StopBits[] StopBitsOptions { get; } = [StopBits.One, StopBits.OnePointFive, StopBits.Two];
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
    public partial Parity Parity { get; set; } = Parity.None;

    [ObservableProperty]
    public partial int DataBits { get; set; } = 8;

    [ObservableProperty]
    public partial StopBits StopBits { get; set; } = StopBits.One;


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
    partial void OnParityChanged(Parity value)
    {
        SerialTransportDescriptor.ValidateParity(value, out var errorMessage);
        TrySetPropertyError(nameof(Parity), errorMessage);
    }
    partial void OnDataBitsChanged(int value)
    {
        SerialTransportDescriptor.ValidateDataBits(value, out var errorMessage);
        TrySetPropertyError(nameof(DataBits), errorMessage);
    }
    partial void OnStopBitsChanged(StopBits value)
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