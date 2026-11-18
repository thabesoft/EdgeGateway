using System.Collections;
using ThabeSoft.EdgeGateway.Codecs.Bits;
using ThabeSoft.EdgeGateway.Codecs.DWords;
using ThabeSoft.EdgeGateway.Codecs.QWords;
using ThabeSoft.EdgeGateway.Codecs.Words;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.DataTypes;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Codecs;


internal sealed class CodecDescriptorConfigMapper :
    IConfigMapper<CodecDescriptor<ushort>>, IConfigMapper<CodecDescriptor<short>>, IConfigMapper<CodecDescriptor<char>>,
    IConfigMapper<CodecDescriptor<uint>>, IConfigMapper<CodecDescriptor<int>>, IConfigMapper<CodecDescriptor<float>>,
    IConfigMapper<CodecDescriptor<ulong>>, IConfigMapper<CodecDescriptor<long>>, IConfigMapper<CodecDescriptor<double>>,

    IConfigMapper<CodecDescriptor<bool>>, IConfigMapper<CodecDescriptor<byte>>, IConfigMapper<CodecDescriptor<BitArray>>
{
    public bool CanMap(IConfigNode root)
    {
        if (!Enum.TryParse<DataType>(root.GetStringValue("DataType"), true, out var enumValue))
            return false;

        return enumValue is DataType.Int32 or DataType.Int32 or DataType.Float;
    }

    CodecDescriptor<byte> IConfigMapper<CodecDescriptor<byte>>.Map(IConfigNode root) =>
        CodecBuilder.Byte;
    CodecDescriptor<bool> IConfigMapper<CodecDescriptor<bool>>.Map(IConfigNode root) =>
       CodecBuilder.Bool(GetBoolBitOffset(root));

    CodecDescriptor<ushort> IConfigMapper<CodecDescriptor<ushort>>.Map(IConfigNode root) =>
      CodecBuilder.UInt16(GetWordEndianess(root));
    CodecDescriptor<short> IConfigMapper<CodecDescriptor<short>>.Map(IConfigNode root) =>
       CodecBuilder.Int16(GetWordEndianess(root));
    CodecDescriptor<char> IConfigMapper<CodecDescriptor<char>>.Map(IConfigNode root) =>
       CodecBuilder.Char(GetWordEndianess(root));


    CodecDescriptor<uint> IConfigMapper<CodecDescriptor<uint>>.Map(IConfigNode root) =>
       CodecBuilder.UInt32(GetDWordEndianess(root));
    CodecDescriptor<int> IConfigMapper<CodecDescriptor<int>>.Map(IConfigNode root) =>
       CodecBuilder.Int32(GetDWordEndianess(root));
    CodecDescriptor<float> IConfigMapper<CodecDescriptor<float>>.Map(IConfigNode root) =>
       CodecBuilder.Float(GetDWordEndianess(root));


    CodecDescriptor<ulong> IConfigMapper<CodecDescriptor<ulong>>.Map(IConfigNode root) =>
      CodecBuilder.UInt64(GetQWordEndianess(root));
    CodecDescriptor<long> IConfigMapper<CodecDescriptor<long>>.Map(IConfigNode root) =>
       CodecBuilder.Int64(GetQWordEndianess(root));
    CodecDescriptor<double> IConfigMapper<CodecDescriptor<double>>.Map(IConfigNode root) =>
       CodecBuilder.Double(GetQWordEndianess(root));


    CodecDescriptor<BitArray> IConfigMapper<CodecDescriptor<BitArray>>.Map(IConfigNode root) =>
       CodecBuilder.Bits(GetBitCount(root), GetBitOrder(root));



    private static int GetBoolBitOffset(IConfigNode node)
    {
        return node.GetStringValue("BitOffset").Required("缺少位偏移")
            .ToInt32();
    }
    private static BitOrder GetBitOrder(IConfigNode node)
    {
        return node.GetStringValue("BitOrder").Required("缺少位序")
            .ToEnum<BitOrder>();
    }
    private static int GetBitCount(IConfigNode node)
    {
        return node.GetStringValue("BitCount").Required("缺少位数")
            .ToInt32();
    }


    private static WordEndianess GetWordEndianess(IConfigNode node)
    {
        return node.GetStringValue("Endianess").Required("缺少字节序")
            .ToEnum<WordEndianess>();
    }
    private static DWordLayout GetDWordEndianess(IConfigNode node)
    {
        return node.GetStringValue("Layout").Required("缺少字节布局")
            .ToEnum<DWordLayout>();
    }
    private static IEnumerable<QWordSwap> GetQWordEndianess(IConfigNode node)
    {
        foreach (var i in node.GetChildren("Swaps"))
        {
            var str = i.AsString();
            if (string.IsNullOrWhiteSpace(str)) continue;

            yield return str.ToEnum<QWordSwap>();
        }
    }
}