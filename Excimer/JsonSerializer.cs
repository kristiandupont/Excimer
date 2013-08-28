using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Linq;

namespace Excimer
{
    public static class JsonSerializer
    {
        public static string Serialize(object rawResult)
        {
            if (rawResult == null)
            {
                return "null";
            }
            if (rawResult is bool)  // Booleans must return "false" rather than "False"
            {
                return rawResult.ToString().ToLower();
            }
            else if (rawResult is double || rawResult is float)   // floating points must be with . 
            {
                return String.Format(CultureInfo.InvariantCulture, "{0}", rawResult);
            }
            else if (rawResult.GetType().IsPrimitive)   // Other primitive types just return the result. 
            {
                return rawResult.ToString();
            }
            else if (rawResult is string)   // String. Return the escaped result in quotation marks
            {
                return String.Format("\"{0}\"", Escape((string)rawResult));
            }
            else if (rawResult is DateTime)  // DateTime. Return miliseconds since 1970. This can be used in js with new Date().
            {
                return UnixTicks((DateTime)rawResult).ToString();
            }
            else if (rawResult is TimeSpan) // TimeSpan. For now we use minutes as floting point
            {
                return ((TimeSpan)rawResult).TotalMinutes.ToString(CultureInfo.InvariantCulture);
            }
            else if (rawResult is IDictionary) // Dictionary. Create a js map.
            {
                var entries = new List<string>();
                foreach (DictionaryEntry de in (IDictionary)rawResult)
                    entries.Add(String.Format("\"{0}\": {1}", de.Key, Serialize(de.Value)));

                return "{" + String.Join(", ", entries) + "}";
            }
            else if (rawResult is IEnumerable)  // Enumerable. Create an array.
            {
                var entries = (from object e in (IEnumerable) rawResult select Serialize(e));
                return "[" + String.Join(", ", entries) + "]";
            }
            else    // Composite. Store all properties.
            {
                var properties = rawResult.GetType().GetProperties();
                var entries = properties.Select(prop => "\"" + prop.Name.ToLowerCaseFirstLetter() + "\": " + Serialize(prop.GetValue(rawResult, null)));
                return "{" + String.Join(", ", entries) + "}";
            }
        }

        public static string Escape(string s)
        {
            return s.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("/", "\\/")
                .Replace("\b", "\\b")
                .Replace("\t", "\\t")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\f", "\\f");
        }

        // returns the number of milliseconds since unix epoch
        private static long UnixTicks(DateTime dt)
        {
            var epoch = new DateTime(1970, 1, 1);
            var unixTicks = dt - epoch;
            return (long)Math.Floor(unixTicks.TotalMilliseconds);
        }
    }
}
