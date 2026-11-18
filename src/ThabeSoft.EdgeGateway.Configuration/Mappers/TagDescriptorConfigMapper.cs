using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Configuration.Mappers;

public sealed class TagDescriptorConfigMapper<T>(
        IConfigMapper<CodecDescriptor<T>> codecMapper,
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
