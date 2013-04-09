using System;
using System.Collections.Generic;
using System.Linq;

namespace ToileDeFond.Modularity
{
    internal static class Extensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
                                                                     Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dic, K key, V defaultVal)
        {
            V ret;
            bool found = dic.TryGetValue(key, out ret);
            if (found)
            {
                return ret;
            }
            return defaultVal;
        }

        public static string PathCombine(this string string1, string string2)
        {
            char separator = '\\';
            const char inversedSeparator = '/';

            if (string1.Contains(inversedSeparator) || string2.Contains(inversedSeparator))
                separator = inversedSeparator;

            var spliter = new[] {separator};

            List<string> words = string1.Split(spliter).ToList();
            words.AddRange(string2.Split(spliter));

            return string.Join(separator.ToString(), words);
        }
    }
}