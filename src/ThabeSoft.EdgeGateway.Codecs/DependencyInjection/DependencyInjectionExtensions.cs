using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections;
using ThabeSoft.EdgeGateway.Codecs.Bits;
using ThabeSoft.EdgeGateway.Codecs.Bytes;
using ThabeSoft.EdgeGateway.Codecs.DWords;
using ThabeSoft.EdgeGateway.Codecs.QWords;
using ThabeSoft.EdgeGateway.Codecs.Words;
using ThabeSoft.EdgeGateway.Configuration;

namespace ThabeSoft.EdgeGateway.Codecs.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGatewayCodec()
        {
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

        public IServiceCollection AddEdgeGatewayCodecConfig()
        {
            descriptors.TryAddSingleton<CodecDescriptorConfigMapper>();
            
            // byte
            descriptors.TryAddSingleton<IConfigMapper<CodecDescriptor<bool>>>(sp => sp.GetRequiredService<CodecDescriptorConfigMapper>());
            descriptors.TryAddSingleton<IConfigMapper<CodecDescriptor<byte>>>(sp => sp.GetRequiredService<CodecDescriptorConfigMapper>());

            // word
            descriptors.TryAddSingleton<IConfigMapper<CodecDescriptor<ushort>>>(sp => sp.GetRequiredService<CodecDescriptorConfigMapper>()); 
            descriptors.TryAddSingleton<IConfigMapper<CodecDescriptor<short>>>(sp => sp.GetRequiredService<CodecDescriptorConfigMapper>());
            descriptors.TryAddSingleton<IConfigMapper<CodecDescriptor<char>>>(sp => sp.GetRequiredService<CodecDescriptorConfigMapper>());

            // dword
            descriptors.TryAddSingleton<IConfigMapper<CodecDescriptor<int>>>(sp => sp.GetRequiredService<CodecDescriptorConfigMapper>());
            descriptors.TryAddSingleton<IConfigMapper<CodecDescriptor<uint>>>(sp => sp.GetRequiredService<CodecDescriptorConfigMapper>());   
            descriptors.TryAddSingleton<IConfigMapper<CodecDescriptor<float>>>(sp => sp.GetRequiredService<CodecDescriptorConfigMapper>());

            // qword
            descriptors.TryAddSingleton<IConfigMapper<CodecDescriptor<double>>>(sp => sp.GetRequiredService<CodecDescriptorConfigMapper>());
            descriptors.TryAddSingleton<IConfigMapper<CodecDescriptor<ulong>>>(sp => sp.GetRequiredService<CodecDescriptorConfigMapper>());
            descriptors.TryAddSingleton<IConfigMapper<CodecDescriptor<long>>>(sp => sp.GetRequiredService<CodecDescriptorConfigMapper>());

            // bits
            descriptors.TryAddSingleton<IConfigMapper<CodecDescriptor<BitArray>>>(sp => sp.GetRequiredService<CodecDescriptorConfigMapper>());


            return descriptors;
        }
    }
}