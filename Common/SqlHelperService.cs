using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

public class SqlHelperService
{
    private readonly string _connectionString;

    public SqlHelperService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // Execute SP and return list
    public async Task<List<T>> ExecuteStoredProcedureAsync<T>(string procedureName, DynamicParameters parameters)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var result = await connection.QueryAsync<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
    }

    // Execute SP and return DataTable (DynamicParameters)
    public async Task<DataTable> ExecuteStoredProcedureToDataTableAsync(string procedureName, DynamicParameters parameters)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                foreach (var paramName in parameters.ParameterNames)
                {
                    var value = parameters.Get<object>(paramName);
                    command.Parameters.AddWithValue(paramName, value ?? DBNull.Value);
                }

                using (var adapter = new SqlDataAdapter(command))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
    }

    // Execute SP and return DataTable (SqlParameter[])
    public async Task<DataTable> ExecuteStoredProcedureToDataTableAsync(string procedureName, SqlParameter[] parameters)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                using (var adapter = new SqlDataAdapter(command))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
    }

    // SQL text command to List<T>
    public async Task<List<T>> ExecuteSqlQueryAsync<T>(string sql)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var result = await connection.QueryAsync<T>(sql);
            return result.ToList();
        }
    }

    // SQL text command to DataTable
    public async Task<DataTable> ExecuteSqlQueryToDataTableAsync(string sql)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(sql, connection))
            {
                using (var adapter = new SqlDataAdapter(command))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
    }
}