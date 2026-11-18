using System.Diagnostics;

namespace ThabeSoft.EdgeGateway.Configuration;


public interface IConfigNode
{
    string GetPath();

    /// <summary>
    /// 根节点
    /// </summary>
    IConfigNode GetRoot();

    /// <summary>
    /// 父节点
    /// </summary>
    IConfigNode? GetParent();


    IConfigNode? GetChild(string key);
    IConfigNode? GetChild(int index);


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
        [DebuggerHidden]
        public IConfigNode? GetChild(string path)
        {
            return node.GetChild(path);
        }

        /// <summary>
        /// 获取当前路径下的字符串属性值
        /// </summary>
        [DebuggerHidden]
        public string? GetStringValue(string path)
        {
            return node.GetChild(path)?.AsString();
        }

        [DebuggerHidden]
        public IEnumerable<IConfigNode> GetChildren(string path)
        {
            return node.GetChild(path)?.AsArray() ?? [];
        }
    }
}