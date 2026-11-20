namespace ThabeSoft.Configuration.Nodes;


public interface IConfigNodeParser
{
    ValueTask<IConfigNode> ParseAsync(Stream stream, CancellationToken cancellationToken = default);
}