
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MESWebDev.Data;

namespace MESWebDev.Common
{
    public class Procedure
    {
        private readonly AppDbContext _con;
     
        public Procedure(AppDbContext con)
        {
            _con = con;
        }

        public async Task<DataTable> Proc_GetDatatable(string proc_name ,Dictionary<string,object> dic)
        {
            DataTable dt = new();
            using (var command = _con.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = proc_name;// dic[SD.ProcedureName.Name].ToString();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;

                //dic = dic.Where(i => i.Key != SD.ProcedureName.Name).ToDictionary(i=>i.Key,i=>i.Value);
                foreach (var item in dic)
                {
                    if(item.Value!=null && !string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        command.Parameters.Add(new SqlParameter(item.Key, item.Value));                      
                    }
                }

                _con.Database.OpenConnection();
                try
                {
                    using(var result = command.ExecuteReader())
                    {
                        dt.Load(result);
                    }
                }
                catch (Exception ex)
                {
                    dt = new();
                }
            }
            return dt;
        }

        public async Task<DataSet> Proc_GetDataset(string proc_name, Dictionary<string, object> dic)
        {
            DataSet ds = new();
            using(var command = _con.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = proc_name;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                foreach(var d in dic)
                {
                    command.Parameters.Add(new SqlParameter (d.Key, d.Value));
                }

                _con.Database.OpenConnection();

                try
                {
                    using(SqlDataAdapter da = new SqlDataAdapter())
                    {
                        da.SelectCommand = (SqlCommand)command;
                        da.Fill(ds);
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    _con.Database.CloseConnection();
                }
            }
            return ds;
        }


        
        public async Task<string> Proc_GetString(string proc_name, Dictionary<string, object> dic)
        {
            string value = string.Empty;
            using (var command = _con.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = proc_name;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                foreach (var d in dic)
                {
                    command.Parameters.Add(new SqlParameter(d.Key, d.Value));
                }

                _con.Database.OpenConnection();

           
                try
                {
                    var result = await command.ExecuteScalarAsync();
                    if (result != null)
                    {
                        value = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    _con.Database.CloseConnection();
                }
            }
            return value;
        }

        public async Task Proc_ExcecuteNonQuery(string proc_name, Dictionary<string, object> dic)
        {
            string value = string.Empty;
            using (var command = _con.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = proc_name;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                foreach (var d in dic)
                {
                    command.Parameters.Add(new SqlParameter(d.Key, d.Value));
                }

                _con.Database.OpenConnection();
                await command.ExecuteNonQueryAsync();
                _con.Database.CloseConnection();                
            }
        }

    }
}
