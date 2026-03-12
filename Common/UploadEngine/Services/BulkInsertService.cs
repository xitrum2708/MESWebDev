using MESWebDev.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MESWebDev.Common.UploadEngine.Services
{
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;              // Recommended for EF Core SQL Server
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class BulkInsertService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;       // EF Core DbContext
        private readonly ILogger<BulkInsertService> _logger;

        public BulkInsertService(AppDbContext db, IConfiguration config, ILogger<BulkInsertService> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
        }

        // <summary>
        // Bulk insert a DataTable into a target table. Column names in the DataTable
        // must match the destination columns (case-insensitive). Use dbo.Table if schema not provided.
        // </summary>
        public async Task BulkInsertAsync(string tableName, DataTable table, CancellationToken ct = default)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentException("tableName is required", nameof(tableName));
            if (table.Rows.Count == 0) { _logger.LogInformation("DataTable is empty. Nothing to insert."); return; }

            // Ensure schema-qualified name
            var destName = tableName.Contains(".") ? tableName : $"dbo.{tableName}";

            // Get underlying SqlConnection from DbContext
            var dbConn = _db.Database.GetDbConnection();
            if (dbConn is not SqlConnection sqlConn)
                throw new InvalidOperationException("Current provider is not SQL Server (SqlConnection required for SqlBulkCopy).");

            // Reuse current EF transaction if present
            SqlTransaction? sqlTran = null;
            var currentTran = _db.Database.CurrentTransaction;
            if (currentTran != null)
                sqlTran = (SqlTransaction)currentTran.GetDbTransaction();

            var shouldClose = false;
            try
            {
                if (sqlConn.State != ConnectionState.Open)
                {
                    await sqlConn.OpenAsync(ct);
                    shouldClose = true;
                }

                // Choose options as needed (KeepIdentity/CheckConstraints/FireTriggers/TableLock ...)
                var options = SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.TableLock;

                using var bulk = new SqlBulkCopy(sqlConn, options, sqlTran)
                {
                    DestinationTableName = destName,   // e.g. dbo.MyTable
                    BatchSize = 1000,
                    BulkCopyTimeout = 0,              // no timeout
                    EnableStreaming = true
                };

                // Column mappings: DataTable ColumnName -> Destination column with same name
                foreach (DataColumn col in table.Columns)
                {
                    bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }

                // Push data
                await bulk.WriteToServerAsync(table, ct);

                _logger.LogInformation("Bulk insert to {Table} completed. Rows: {Rows}", destName, table.Rows.Count);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Bulk insert to {Table} failed with SqlException: {Message}", tableName, ex.Message);
                throw; // rethrow so caller can handle/rollback if needed
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk insert to {Table} failed: {Message}", tableName, ex.Message);
                throw;
            }
            finally
            {
                // If we opened the connection here and no ambient transaction needs it, close it
                if (shouldClose && sqlConn.State == ConnectionState.Open && _db.Database.CurrentTransaction == null)
                    await sqlConn.CloseAsync();
            }
        }
    }
}
