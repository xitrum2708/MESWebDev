using MESWebDev.Data;
using MESWebDev.Models.Setting;
using MESWebDev.Models.Setting.DTO;
using MESWebDev.Services.IService;
using System.Data;
using System.Globalization;
using System.Runtime.Serialization;

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

        public static string FormatCell(object v, Type t, FormatRazorDTO fr)
        {
            if (v == null || v is DBNull) return "";
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (IsNumeric(t) && v is IFormattable f)
                return f.ToString(fr.NumberFormat, culture);
            // date or datetime

            if (t == typeof(DateTime))
            {
                DateTime dt = (DateTime)v;
                if (dt.TimeOfDay.TotalSeconds == 0)
                    return dt.ToString(fr.DateFormat, culture);
                return ((DateTime)v).ToString(fr.DatetimeFormat, culture);
            }                
            return v.ToString();
        }

        public static string FormatCell2(object v, Type t)
        {
            if (v == null || v is DBNull) return "";
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (IsNumeric(t) && v is IFormattable f)
                return f.ToString("#,0.##########", culture);
            // date or datetime

            if (t == typeof(DateTime))
            {
                DateTime dt = (DateTime)v;
                if (dt.TimeOfDay.TotalSeconds == 0)
                    return dt.ToString("yyyy/MM/dd", culture);
                return ((DateTime)v).ToString("yyyy/MM/dd HH:mm:ss", culture);
            }
            return v.ToString();
        }

        public static string FormatCss(Type t, FormatRazorDTO fr)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (IsNumeric(t))
                return fr.NumberCss;
            if (t == typeof(DateTime))
                return fr.DateCss;
            return fr.TextCss;
        }

        public static string FormatCss2(Type t)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (IsNumeric(t))
                return "text-end";
            if (t == typeof(DateTime))
                return "text-center";
            return "text-start";
        }

        public static T GetValueOrDefault<T>(IDataReader reader, Dictionary<string, int> colMap, string colName, T defaultValue = default)
        {
            if (!colMap.TryGetValue(colName, out int idx))
                return defaultValue;

            object val = reader.GetValue(idx);
            if (val == DBNull.Value || val == null)
                return defaultValue;

            return (T)Convert.ChangeType(val, typeof(T));
        }

    }

}
