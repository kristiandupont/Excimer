using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excimer
{
    public static class StringExtensions
    {
        public static string ToLowerCaseFirstLetter(this string s)
        {
            if (String.IsNullOrEmpty(s)) return s;
            var firstLetter = s[0];
            return firstLetter.ToString().ToLower() + s.Substring(1);
        }

        public static string ConcatenateWithSeparator(string firstPart, string separator, string secondPart)
        {
            if (!String.IsNullOrEmpty(firstPart) && !String.IsNullOrEmpty(secondPart))
                return firstPart + separator + secondPart;
            else if (!String.IsNullOrEmpty(firstPart))
                return firstPart;
            else // if(!string.IsNullOrEmpty(secondPart))
                return secondPart;
        }
    }
}
