using System.Collections.Generic;

namespace LiteHelper.Model;

/// <summary>
/// Compares instances of <see cref="IRecord"/> for sorting.
/// </summary>
public class RecordComparer : IComparer<IRecord>
{
    /// <summary>
    /// Compares two instances of <see cref="IRecord"/> and returns an indication of their relative sort order.
    /// </summary>
    /// <param name="x">The first <see cref="IRecord"/> to compare.</param>
    /// <param name="y">The second <see cref="IRecord"/> to compare.</param>
    /// <returns>
    /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>:
    /// -1 if <paramref name="x"/> is less than <paramref name="y"/>,
    /// 0 if <paramref name="x"/> is equal to <paramref name="y"/>,
    /// 1 if <paramref name="x"/> is greater than <paramref name="y"/>.
    /// </returns>
    public int Compare(IRecord? x, IRecord? y) => x switch
    {
        null when y == null => 0,
        null => -1,
        _ => y == null ? 1 : x.Id.CompareTo(y.Id)
    };
}