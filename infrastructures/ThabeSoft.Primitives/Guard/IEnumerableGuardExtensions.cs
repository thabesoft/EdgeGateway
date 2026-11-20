namespace ThabeSoft.Primitives.Guard;


public static class IEnumerableGuardExtensions
{
    /// <summary>
    /// 筛选非空元素
    /// </summary>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> values)
        where T : notnull
    {
        if (values is null) return [];
        return values.Where(x => x is not null).Select(x => x!);
    }
}
