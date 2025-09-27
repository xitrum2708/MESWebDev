using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System.Data;

namespace MESWebDev.Common
{
    public static class SessionExtension
    {
        public static T GetComplexData<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            if (data == null) { return default(T); }
            else return JsonConvert.DeserializeObject<T>(data);
        }

        public static void SetComplexData(this ISession session, string key, object data)
        {
            session.SetString(key, JsonConvert.SerializeObject(data));
        }



        public static void SetComplexDatatable(this ISession session, string key, object data)
        {
            session.SetString(key, JsonConvert.SerializeObject(data, Formatting.Indented));
        }
        public static DataTable GetComplexDatatable(this ISession session, string key)
        {
            var data = session.GetString(key);
            if (data == null) return new DataTable();
            else return JsonConvert.DeserializeObject<DataTable>(data);
        }
    }
}
