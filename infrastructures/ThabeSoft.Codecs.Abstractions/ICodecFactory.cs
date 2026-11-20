namespace ThabeSoft.EdgeGateway.Codecs;

public interface ICodecFactory
{
    ICodec<T> Create<T>(CodecDescriptor<T> descriptor) where T : notnull;
}