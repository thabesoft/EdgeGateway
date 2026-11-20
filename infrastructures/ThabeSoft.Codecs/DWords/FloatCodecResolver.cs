using ThabeSoft.Codecs.Extensions;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.Codecs.DWords;


internal sealed class FloatCodecResolver : ICodecResolver<float>
{
    public bool CanResolve(CodecDescriptor<float> descriptor) => true;

    public ICodec<float> Resolve(CodecDescriptor<float> descriptor)
    {
        var opts = descriptor.AssertType<FloatCodecDescriptor>();
        return DWordCodec.Create(opts.Layout).AsFloat();
    }
}