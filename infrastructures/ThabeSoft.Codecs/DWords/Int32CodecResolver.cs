using ThabeSoft.Codecs.Extensions;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.Codecs.DWords;


internal sealed class Int32CodecResolver : ICodecResolver<int>
{
    public bool CanResolve(CodecDescriptor<int> descriptor) => true;

    public ICodec<int> Resolve(CodecDescriptor<int> descriptor)
    {
        var opts = descriptor.AssertType<UInt32CodecDescriptor>();
        return DWordCodec.Create(opts.Layout).AsInt();
    }
}
