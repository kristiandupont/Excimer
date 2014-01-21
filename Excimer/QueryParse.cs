using System;
using System.Globalization;
using Excimer.Drawing;

namespace Excimer
{
    public static class QueryParse
    {
        public static object Parse(string v, Type type)
        {
            if (type == typeof(string)) return v;
            else if (type == typeof(int)) return int.Parse(v);
            else if (type == typeof(double)) return double.Parse(v, CultureInfo.InvariantCulture);
            else if (type == typeof(bool)) return bool.Parse(v);
            else if (type == typeof(DateTime)) return DateTime.ParseExact(v, DateFormat, CultureInfo.InvariantCulture);
            else if (type == typeof(TimeSpan)) return TimeSpan.Parse(v, CultureInfo.InvariantCulture);
            else if (type == typeof(Color)) return Color.FromHtml("#" + v);
            else throw new ArgumentException("Unknown type '" + type.Name + "'.");
        }

        public static string DateFormat { get { return "yyyy-MM-dd HH:mm:ss.FFFFFFF"; } }
    }
}
