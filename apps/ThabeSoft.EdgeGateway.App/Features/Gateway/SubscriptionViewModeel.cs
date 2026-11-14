using CommunityToolkit.Mvvm.ComponentModel;
using SukiUI.Toasts;
using System.Reactive.Linq;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Subscription;

using static System.ObservableExtensions;

namespace ThabeSoft.EdgeGateway.App.Features.Gateway;


internal sealed partial class SubscriptionViewModel : LifecycleViewModel, IDisposable
{
    private readonly ISubscription _subscription;
    private readonly IDisposable _subscriptionHandle;
    private readonly RingBuffer<double> _buffer = new(120);

    public SubscriptionViewModel(ISubscription subscription, ISukiToastManager sukiToastManager) : base(subscription, sukiToastManager)
    {
        _subscription = subscription;
        Latest = subscription.Latest;
        _subscriptionHandle = subscription.Update
            .ObserveOnUI()
            .Subscribe(
                onNext: value =>
                {
                    Latest = value;

                    if (value.Value is double d)
                        _buffer.Write(d);
                },
                onError: _ => Latest = Reading.Bad(Latest?.Value ?? default!),
                onCompleted: delegate { });
    }

    public SubscriptionId Id => _subscription.Id;
    public IDataAddress Address => _subscription.Address;
    [ObservableProperty] public partial Reading? Latest { get; set; }
    public RingBuffer<double> Buffer => _buffer;


    public void Dispose()
    {
        _subscriptionHandle.Dispose();
    }
}




public sealed class RingBuffer<T>
{
    private readonly T[] _buffer;
    private readonly object _gate = new();
    private int _index;        // 下一个写入位置
    private int _count;        // 已写入数量（≤ _buffer.Length）

    public RingBuffer(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);
        _buffer = new T[capacity];
    }

    public int Capacity => _buffer.Length;

    /// <summary>当前已存元素数量（≤ Capacity）。</summary>
    public int Count
    {
        get { lock (_gate) return _count; }
    }

    /// <summary>写入一个元素，覆盖最旧。</summary>
    public void Write(T item)
    {
        lock (_gate)
        {
            _buffer[_index] = item;
            _index = (_index + 1) % _buffer.Length;
            if (_count < _buffer.Length) _count++;
        }
    }

    /// <summary>
    /// 按「从最旧到最新」的顺序拷出当前快照。
    /// 每次调用分配一个新数组（调用方按刷新频率调用，通常 10 FPS，可接受）。
    /// </summary>
    public T[] Snapshot()
    {
        lock (_gate)
        {
            var result = new T[_count];
            if (_count == 0) return result;

            // 最旧元素的位置：未满时是 0，已满时是 _index（下一个写入位置 = 最旧）
            var start = _count < _buffer.Length ? 0 : _index;
            for (int i = 0; i < _count; i++)
                result[i] = _buffer[(start + i) % _buffer.Length];

            return result;
        }
    }

    /// <summary>清空（不释放数组）。</summary>
    public void Clear()
    {
        lock (_gate)
        {
            Array.Clear(_buffer);
            _index = 0;
            _count = 0;
        }
    }
}