using System.Text;

namespace ThabeSoft.Configuration;


public interface IConfigSerializer
{
    ValueTask SerializeAsync(object config, Stream stream, CancellationToken cancellationToken = default);
    ValueTask<object> DeserializeAsync(Stream stream, CancellationToken cancellationToken = default);
}



public static class ConfigSerializerExtensions
{
    extension(IConfigSerializer serializer)
    {
        public async ValueTask<object> DeserializeFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            using var fs = File.OpenRead(filePath);
            return await serializer.DeserializeAsync(fs, cancellationToken);
        }

        public async ValueTask<object> DeserializeStringAsync<T>(string str, CancellationToken cancellationToken = default)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            using var ms = new MemoryStream(bytes);

            return await serializer.DeserializeAsync(ms, cancellationToken);
        }
    }
}