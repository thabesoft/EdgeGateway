#if NETSTANDARD2_0

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace System;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

public static class SystemExtensions
{
    extension(Enum)
    {
        public static bool IsDefined<T>(T value) where T : Enum
        {
            return Enum.IsDefined(typeof(T), value);
        }


        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static T Parse<T>(string value) where T : struct, Enum
        {
            return Parse<T>(value, false);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static T Parse<T>(string value, bool ignoreCase) where T : struct, Enum
        {
            if (value is null) throw new ArgumentNullException(nameof(value), "枚举值不可为空");
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("枚举值不可为空白", nameof(value));
            if (!Enum.TryParse<T>(value, ignoreCase, out var result)) throw new ArgumentException($"枚举值不存在: {value}", nameof(value));

            return result;
        }
    }
}

#endif