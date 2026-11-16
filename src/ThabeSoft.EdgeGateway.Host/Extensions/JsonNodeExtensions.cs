using System.Text.Json.Nodes;

namespace ThabeSoft.EdgeGateway.Extensions;


internal static class JsonNodeExtensions
{
    extension(JsonNode node)
    {
        public JsonNode? GetByPath(params ReadOnlySpan<string> paths)
        {
            var current = node;

            foreach (var path in paths)
            {
                current = current[path];
                if (current is null) return null;
            }

            return current;
        }

        public string? GetValueStringByPath(params ReadOnlySpan<string> paths)
        {
            return node.GetByPath(paths)?.ToString();
        }
    }
}
