using System.Diagnostics;

namespace ThabeSoft.EdgeGateway.Configuration.Nodes;


public interface IConfigNode
{
    ConfigNodeType Type { get; }
    IConfigNode Root { get; }
    IConfigNode? Parent { get; }



    IConfigNode? GetByPath(string path);


    /// <summary>
    /// 获取当前节点路径
    /// </summary>
    string GetPath();
}


public static class ConfigNodeExtensions
{
    // AsValu
    extension(IConfigNode node)
    {
        [DebuggerHidden]
        public IConfigValueNode? AsValue()
        {
            return node as IConfigValueNode;
        }

        [DebuggerHidden]
        public string? AsString()
        {
            return node.AsValue()?.GetString();
        }
    }

    // Array
    extension(IConfigNode node)
    {
        [DebuggerHidden]
        public IConfigArrayNode? AsArray()
        {
            return node as IConfigArrayNode;
        }
    }

    // Object
    extension(IConfigNode node)
    {
        [DebuggerHidden]
        public IConfigObjectNode? AsObject()
        {
            return node as IConfigObjectNode;
        }
    }

    extension(IConfigNode node)
    {
        [DebuggerHidden]
        public string? GetStringByPath(string path)
        {
            return node.GetByPath(path)?.AsString();
        }

        [DebuggerHidden]
        public IEnumerable<IConfigNode> EnumerateChildrenByPath(string path)
        {
            return node.GetByPath(path) switch
            {
                IConfigArrayNode arr => arr,
                IConfigObjectNode obj => obj.Select(x => x.Value),
                _ => []
            };
        }
    }
}