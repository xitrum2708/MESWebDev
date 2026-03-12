using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace MESWebDev.Common
{

    public class DatatableListMap
    {
        //------ Convert from Datatable to List ----------------------------\\
        public List<T> ConvertToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase); // faster lookup

            var properties = typeof(T).GetProperties();

            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();

                foreach (var pro in properties)
                {
                    if (!columnNames.Contains(pro.Name))
                        continue;

                    var value = row[pro.Name];

                    if (value == DBNull.Value)
                    {
                        pro.SetValue(objT, null);
                        continue;
                    }

                    // Handle Nullable<T>
                    var targetType = Nullable.GetUnderlyingType(pro.PropertyType)
                                     ?? pro.PropertyType;

                    object safeValue;

                    if (targetType.IsEnum)
                    {
                        safeValue = Enum.ToObject(targetType, value);
                    }
                    else
                    {
                        safeValue = Convert.ChangeType(value, targetType);
                    }

                    pro.SetValue(objT, safeValue);
                }

                return objT;
            }).ToList();
        }

        //public List<T> ConvertToList<T>(DataTable dt)
        //{

        //    var columnNames = dt.Columns.Cast<DataColumn>()
        //            .Select(c => c.ColumnName)
        //            .ToList();
        //    var properties = typeof(T).GetProperties();
        //    return dt.AsEnumerable().Select(row =>
        //    {
        //        var objT = Activator.CreateInstance<T>();
        //        foreach (var pro in properties)
        //        {
        //            if (columnNames.Contains(pro.Name))
        //            {
        //                PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
        //                pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], pI.PropertyType));
        //            }
        //        }
        //        return objT;
        //    }).ToList();
        //}


        //-------------- Convert List to Datatable --------------\\
        public DataTable ToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                string t = prop.PropertyType.ToString();
                if (prop.Name.Contains("_dt"))
                {
                    string km = "";
                }
                //t = prop.DisplayName;
                if (t.ToLower().Contains("date"))
                {
                    table.Columns.Add(prop.DisplayName == null ? prop.Name : prop.DisplayName, typeof(string));
                }
                else
                    table.Columns.Add(prop.DisplayName == null ? prop.Name : prop.DisplayName, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    string t = props[i].PropertyType.ToString();
                    if (props[i].Name.Contains("_dt"))
                    {
                        string km = "";
                    }
                    if (t.ToLower().Contains("date"))
                    {
                        if (props[i].GetValue(item) != null)
                        {
                            values[i] = Convert.ToDateTime(props[i].GetValue(item)) == DateTime.MinValue ? "-" : " " + Convert.ToDateTime(props[i].GetValue(item)).ToString("yyyy/MM/dd");
                        }
                    }
                    else
                        values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

    }
}
