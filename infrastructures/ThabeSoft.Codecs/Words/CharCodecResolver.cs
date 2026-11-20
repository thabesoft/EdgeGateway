using ThabeSoft.EdgeGateway.Codecs.Extensions;
using ThabeSoft.EdgeGateway.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Codecs.Words;

internal sealed class CharCodecResolver : ICodecResolver<char>
{
    public bool CanResolve(CodecDescriptor<char> descriptor) => true;

    public ICodec<char> Resolve(CodecDescriptor<char> descriptor)
    {
        var opts = descriptor.AssertType<UInt16CodecDescriptor>();
        return WordCodec.Create(opts.Endianess).AsChar();
    }
}