namespace ThabeSoft.EdgeGateway.Configuration;

public interface IConfigNodeParser
{
    ValueTask<IConfigNode> ParseAsync(Stream stream, CancellationToken cancellationToken = default);
}