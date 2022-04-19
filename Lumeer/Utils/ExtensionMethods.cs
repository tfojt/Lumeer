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
    }
}
