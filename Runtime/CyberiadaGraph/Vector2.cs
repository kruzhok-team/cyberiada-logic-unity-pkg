using System;
using System.Globalization;

#if !UNITY
namespace Talent.Graphs
{
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 zero => new Vector2(0, 0);

        public string ToString(string format)
        {
            IFormatProvider formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            return $"{x.ToString(format, formatProvider)}, {y.ToString(format, formatProvider)}";
        }
    }
}
#endif