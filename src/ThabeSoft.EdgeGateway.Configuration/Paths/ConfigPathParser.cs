namespace ThabeSoft.EdgeGateway.Configuration.Paths;

internal sealed class ConfigPathParser : IConfigPathParser
{
    public IEnumerable<IConfigPathToken> Parse(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            yield break;

        int i = 0;
        int len = path.Length;

        while (i < len)
        {
            char c = path[i];

            // Parent ..
            if (c == '.' && i + 1 < len && path[i + 1] == '.')
            {
                yield return new ConfigParentPathToken();
                i += 2;
                continue;
            }

            // Root $
            if (c == '$')
            {
                yield return new ConfigRootPathToken();
                i++;
                continue;
            }

            // 跳过分隔点
            if (c == '.')
            {
                i++;
                continue;
            }

            // Index [0]
            if (c == '[')
            {
                i++;
                int start = i;
                while (i < len && char.IsAsciiDigit(path[i]))
                    i++;
                string numStr = path[start..i];
                if (i >= len || path[i] != ']')
                    throw new FormatException($"路径下标语法错误，位置{i}：{path}");
                i++; // skip ]
                int idx = int.Parse(numStr);
                yield return new ConfigIndexPathToken(idx);
                continue;
            }

            // Name 标识符：字母、数字、下划线
            if (char.IsLetterOrDigit(c) || c == '_')
            {
                int start = i;
                while (i < len && (char.IsLetterOrDigit(path[i]) || path[i] == '_'))
                    i++;
                string name = path[start..i];
                yield return new ConfigNamePathToken(name);
                continue;
            }

            throw new FormatException($"无法识别路径字符 '{c}' 位置{i}：{path}");
        }
    }
}