using System.Text;

namespace ThabeSoft.EdgeGateway.Configuration;


public interface IConfigSerializer
{
    ValueTask SerializeAsync(GatewayConfig config, Stream stream, CancellationToken cancellationToken = default);
    ValueTask<GatewayConfig> DeserializeAsync(Stream stream, CancellationToken cancellationToken = default);
}



public static class ConfigSerializerExtensions
{
    extension(IConfigSerializer serializer)
    {
        public async Task<GatewayConfig> DeserializeFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            using var fs = File.OpenRead(filePath);
            return await serializer.DeserializeAsync(fs, cancellationToken);
        }

        public async Task<GatewayConfig> DeserializeStringAsync(string str, CancellationToken cancellationToken = default)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            using var ms = new MemoryStream(bytes);

            return await serializer.DeserializeAsync(ms, cancellationToken);
        }
    }
}