namespace ThabeSoft.EdgeGateway.Codecs;


internal interface ICodecResolver<T> where T : notnull
{
    bool CanResolve(CodecDescriptor<T> descriptor);
    ICodec<T> Resolve(CodecDescriptor<T> descriptor);
}