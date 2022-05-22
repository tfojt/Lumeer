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

        public static void AddRange<T>(this ObservableCollection<T> observableCollection, IEnumerable<T> collection, bool clearTargetCollection = false)
        {
            if (clearTargetCollection)
            {
                observableCollection.Clear();
            }

            foreach (var item in collection)
            {
                observableCollection.Add(item);
            }
        }
    }
}
