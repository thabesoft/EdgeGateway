using ThabeSoft.Codecs.Extensions;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.Codecs.QWords;


internal sealed class Int64CodecResolver : ICodecResolver<long>
{
    public bool CanResolve(CodecDescriptor<long> descriptor) => true;

    public ICodec<long> Resolve(CodecDescriptor<long> descriptor)
    {
        var opts = descriptor.AssertType<Int64CodecDescriptor>();
        return QWordCodec.Create(opts.Swaps).AsLong();
    }
}