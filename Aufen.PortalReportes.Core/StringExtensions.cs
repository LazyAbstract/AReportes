using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aufen.PortalReportes.Core
{
    public static class StringExtensions
    {
        public static string Flatten(this IEnumerable<string> list, string separator)
        {
            if (list == null) throw new ArgumentNullException("list");
            if (separator == null) throw new ArgumentNullException("separator");
            string result = list.Aggregate(String.Empty, (acc, next) => acc + next + separator);
            return result.Left(result.Length - separator.Length);
        }

        // ref.: http://stackoverflow.com/questions/844059/net-equivalent-of-the-old-vb-leftstring-length-function
        public static string Left(this string str, int length)
        {
            return str.Substring(0, Math.Min(length, str.Length));
        }

        // ref.: http://stackoverflow.com/questions/844059/net-equivalent-of-the-old-vb-leftstring-length-function
        public static string Right(this string str, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length");

            return str.Substring(Math.Max(0, str.Length - length), Math.Min(length, str.Length));
        }
    }
}
