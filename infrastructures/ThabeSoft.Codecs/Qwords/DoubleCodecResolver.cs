using ThabeSoft.EdgeGateway.Codecs.Extensions;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Codecs.QWords;


internal sealed class DoubleCodecResolver : ICodecResolver<double>
{
    public bool CanResolve(CodecDescriptor<double> descriptor) => true;

    public ICodec<double> Resolve(CodecDescriptor<double> descriptor)
    {
        var opts = descriptor.AssertType<DoubleCodecDescriptor>();
        return QWordCodec.Create(opts.Swaps).AsDouble();
    }
}
