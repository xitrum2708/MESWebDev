using DocumentFormat.OpenXml.CustomProperties;
using System.Data;

namespace MESWebDev.Common.UploadEngine.Helpers
{
    public static class DatatableHelper
    {
        public static DataTable ToDataTable(Type type, List<object> data)
        {
            DataTable dataTable = new();
            var props = type.GetProperties();

            foreach( var p in props)
            {
                dataTable.Columns.Add(p.Name);
            }

            foreach (var item in data)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item)?? DBNull.Value;
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

    }
}
