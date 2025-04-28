using System;
using System.Collections.Generic;

namespace RineaR.Analyzer.Sample;

public static class EnumerableExtensions
{
    public static void FirstOr<T>(this IEnumerable<T> source, Action<T> onExists, Action onNotExists)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (onExists == null) throw new ArgumentNullException(nameof(onExists));
        if (onNotExists == null) throw new ArgumentNullException(nameof(onNotExists));

        using var enumerator = source.GetEnumerator();
        if (enumerator.MoveNext())
        {
            onExists(enumerator.Current);
        }
        else
        {
            onNotExists();
        }
    }

    public static void FirstOr<T>(this IEnumerable<T> source, Func<T, bool> predicate, Action<T> onExists, Action onNotExists)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        if (onExists == null) throw new ArgumentNullException(nameof(onExists));
        if (onNotExists == null) throw new ArgumentNullException(nameof(onNotExists));

        foreach (var item in source)
        {
            if (predicate(item))
            {
                onExists(item);
                return;
            }
        }

        onNotExists();
    }
}