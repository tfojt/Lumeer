using Lumeer.Models.Rest;
using Lumeer.Models.Rest.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Utils
{
    public static class RestParsers
    {
        public static string ParseDateFormat(string restFormat)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < restFormat.Length; i++)
            {
                char ch = restFormat[i];

                if (ch == 'Y')
                {
                    ch = 'y';
                }
                else if (ch == 'D')
                {
                    ch = 'd';
                }

                sb.Append(ch);
            }

            return sb.ToString();
        }
    }
}
