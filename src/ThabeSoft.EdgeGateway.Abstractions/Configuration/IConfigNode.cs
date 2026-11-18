namespace ThabeSoft.EdgeGateway.Configuration;


public interface IConfigNode
{
    /// <summary>
    /// 根节点
    /// </summary>
    IConfigNode Root { get; }

    /// <summary>
    /// 父节点
    /// </summary>
    IConfigNode? Parent { get; }

    /// <summary>
    /// 获取子节点
    /// </summary>
    /// <param name="path">可连续深入层级   a:B:C </param>
    /// <returns></returns>
    IConfigNode? this[string path] { get; }


    string? AsString();
    IEnumerable<IConfigNode> AsArray();
}


public static class ConfigNodeExtensions
{
    extension(IConfigNode node)
    {
        /// <summary>
        /// 获取子节点
        /// </summary>
        public IConfigNode? GetChild(string path)
        {
            return node[path];
        }

        /// <summary>
        /// 获取当前路径下的字符串属性值
        /// </summary>
        public string? GetStringValue(string path)
        {
            return node[path]?.AsString();
        }
        public IEnumerable<IConfigNode> GetChildren(string path)
        {
            return node[path]?.AsArray() ?? [];
        }
    }
}