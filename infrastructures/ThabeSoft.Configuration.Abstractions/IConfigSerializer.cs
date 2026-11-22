using System.Text;

namespace ThabeSoft.Configuration;


public interface IConfigSerializer
{
    ValueTask SerializeAsync<T>(T config, Stream stream, CancellationToken cancellationToken = default);
    ValueTask<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default);
}



public static class ConfigSerializerExtensions
{
    extension(IConfigSerializer serializer)
    {
        public async ValueTask<T> DeserializeFileAsync<T>(string filePath, CancellationToken cancellationToken = default)
        {
            using var fs = File.OpenRead(filePath);
            return await serializer.DeserializeAsync<T>(fs, cancellationToken);
        }

        public async ValueTask<T> DeserializeStringAsync<T>(string str, CancellationToken cancellationToken = default)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            using var ms = new MemoryStream(bytes);

            return await serializer.DeserializeAsync<T>(ms, cancellationToken);
        }
    }
}