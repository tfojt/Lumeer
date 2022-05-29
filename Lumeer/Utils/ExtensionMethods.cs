using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Lumeer.Utils
{
    public static class ExtensionMethods
    {
        public static bool ContainsAny(this string str, params string[] values)
        {
            foreach (var value in values)
            {
                if (str.Contains(value))
                {
                    return true;
                }
            }

            return false;
        }

        public static void AddRange<T>(this ICollection<T> targetCollection, IEnumerable<T> collection, bool clearTargetCollection = false)
        {
            if (clearTargetCollection)
            {
                targetCollection.Clear();
            }

            foreach (var item in collection)
            {
                targetCollection.Add(item);
            }
        }
    }
}
