using System.Globalization;

namespace MESWebDev.Common
{
    public static class CommonFormat
    {
        public static readonly CultureInfo culture = new CultureInfo("en-US");
        public static bool IsNumeric(Type t)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;
            return t == typeof(byte) || t == typeof(sbyte) || t == typeof(short) || t == typeof(ushort)
                || t == typeof(int) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong)
                || t == typeof(float) || t == typeof(double) || t == typeof(decimal);
        }

        public static string FormatCell(object v, Type t)
        {
            if (v == null || v is DBNull) return "";
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (IsNumeric(t) && v is IFormattable f)
                return f.ToString("#,0.##########", culture);
            if (t == typeof(DateTime))
                return ((DateTime)v).ToString("yyyy/MM/dd HH:mm:ss", culture);
            return v.ToString();
        }

    }
}
