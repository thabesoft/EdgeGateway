using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections;
using ThabeSoft.EdgeGateway.Codecs.Bits;
using ThabeSoft.EdgeGateway.Codecs.Bytes;
using ThabeSoft.EdgeGateway.Codecs.DWords;
using ThabeSoft.EdgeGateway.Codecs.QWords;
using ThabeSoft.EdgeGateway.Codecs.Words;

namespace ThabeSoft.EdgeGateway.Codecs.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGatewayCodec()
        {
            // 状态机
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ICodecFactory, CodecFactory>());


            // Byte
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ICodecResolver<bool>, BoolCodecResolver>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ICodecResolver<byte>, ByteCodecResolver>());

            // Word
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ICodecResolver<ushort>, UInt16CodecResolver>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ICodecResolver<short>, Int16CodecResolver>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ICodecResolver<char>, CharCodecResolver>());

            // DWord
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ICodecResolver<uint>, UInt32CodecResolver>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ICodecResolver<int>, Int32CodecResolver>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ICodecResolver<float>, FloatCodecResolver>());

            // Qword
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ICodecResolver<ulong>, UInt64CodecResolver>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ICodecResolver<long>, Int64CodecResolver>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ICodecResolver<double>, DoubleCodecResolver>());

            // Bits
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ICodecResolver<BitArray>, BitsCodecResolver>());


            return descriptors;
        }
    }
}