using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Tags;

public sealed class TagDescriptorConfigMapper<T>(
        IConfigMapper<ICodec<T>> codecMapper,
        IConfigMapper<DataAddress> dataAddressMapper
    ) : IConfigMapper<TagDescriptor<T>>
    where T : notnull
{
    public bool CanMap(IConfigNode root)
    {
        return true;
    }

    public TagDescriptor<T> Map(IConfigNode root)
    {
        var address = dataAddressMapper.Map(root.GetChild("Address").Required("缺少数据地址"));
        var codec = codecMapper.Map(root);

        return new(address, codec);
    }
}
