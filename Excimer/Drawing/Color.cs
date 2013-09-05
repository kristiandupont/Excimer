using System;

namespace Excimer.Drawing
{
    public class Color
    {
        public int A;
        public int R;
        public int G;
        public int B;

        public Color(int a, int r, int g, int b)
        {
            A = Math.Min(255, a);
            R = Math.Min(255, r);
            G = Math.Min(255, g);
            B = Math.Min(255, b);
        }

        public static Color Black { get { return new Color(255, 0, 0, 0); } }

        public static Color White { get { return new Color(255, 255, 255, 255); } }

        public static Color FromHtml(string s)
        {
            int r = 0, g = 0, b = 0; 

            if (s.StartsWith("#"))
            {
                var hexString = s.Substring(1);
                if (hexString.Length == 3)
                {
                    r = Convert.ToInt32(string.Concat(hexString[0], hexString[0]), 16);
                    g = Convert.ToInt32(string.Concat(hexString[1], hexString[1]), 16);
                    b = Convert.ToInt32(string.Concat(hexString[2], hexString[2]), 16);
                }
                else if (hexString.Length == 6)
                {
                    r = Convert.ToInt32(hexString.Substring(0, 2), 16);
                    g = Convert.ToInt32(hexString.Substring(2, 2), 16);
                    b = Convert.ToInt32(hexString.Substring(4, 2), 16);
                }
            }

            return new Color(255, r, g, b);
        }

        public static Color FromArgb(int a, int r, int g, int b)
        {
            return new Color(a, r, g, b);
        }

        public static Color FromRgb(int r, int g, int b)
        {
            return new Color(255, r, g, b);
        }

        public override bool Equals(object obj)
        {
            var c = obj as Color;
            if (c == null) return false;

            return c.A == A && c.R == R && c.G == G && c.B == B;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            var result = string.Format("#{0:X2}{1:X2}{2:X2}", R, G, B);
            if (A != 255)
                result += string.Format("[{0:X2}]", A);
            return result;
        }

        public static Color operator +(Color firstColor, Color otherColor)
        {
            return FromArgb(firstColor.A + otherColor.A, 
                                        firstColor.R + otherColor.R, 
                                        firstColor.G + otherColor.G,
                                        firstColor.B + otherColor.B);
        }

        public static Color operator *(Color color, byte multiplier)
        {
            var factor = multiplier/255.0;
            return FromArgb((int) (color.A*factor), (int) (color.R*factor), (int) (color.G*factor),
                            (int) (color.B*factor));
        }
    }
}
