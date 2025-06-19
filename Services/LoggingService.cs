using MESWebDev.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MESWebDev.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(AppDbContext context, ILogger<LoggingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void LogAction(string actionName, string actionType, Action action, string createdBy = "System", string additionalDetails = null)
        {
            var startTime = DateTime.Now;
            var stopwatch = Stopwatch.StartNew();
            string status = "Success";
            string errorMessage = null;

            try
            {
                action();
            }
            catch (Exception ex)
            {
                status = "Failed";
                errorMessage = ex.Message;
                _logger.LogError(ex, $"Action {actionName} ({actionType}) failed.");
                throw; // Re-throw to let the caller handle the exception
            }
            finally
            {
                stopwatch.Stop();
                var endTime = DateTime.Now;
                var durationMs = (int)stopwatch.ElapsedMilliseconds;
                // Include additional details in the error message or a new column if needed
                var logMessage = additionalDetails != null ? $"{errorMessage ?? ""} | Details: {additionalDetails}" : errorMessage;

                // Log to database
                _context.Database.ExecuteSqlRaw(
                    "EXEC [dbo].[spweb_InsertExecutionLog] @TaskName, @StartTime, @EndTime, @DurationMs, @Status, @ErrorMessage, @CreatedBy",
                    new SqlParameter("@TaskName", $"{actionType}: {actionName}"),
                    new SqlParameter("@StartTime", startTime),
                    new SqlParameter("@EndTime", endTime),
                    new SqlParameter("@DurationMs", durationMs),
                    new SqlParameter("@Status", status),
                    new SqlParameter("@ErrorMessage", logMessage ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedBy", createdBy)
                );

                // Log to application log
                if (status == "Success")
                {
                    _logger.LogInformation($"Action {actionName} ({actionType}) completed in {durationMs}ms.");
                }
            }
        }

        public async Task<T> LogActionAsync<T>(string actionName, string actionType, Func<Task<T>> action, string createdBy = "System", string additionalDetails = null)
        {
            var stopwatch = Stopwatch.StartNew();
            string status = "Success";
            string errorMessage = null;
            T result = default;

            try
            {
                result = await action();
            }
            catch (Exception ex)
            {
                status = "Failed";
                errorMessage = ex.ToString();
                _logger.LogError(ex, $"Action {actionName} ({actionType}) failed.");

                stopwatch.Stop();
                var startTime = DateTime.Now.AddMilliseconds(-stopwatch.ElapsedMilliseconds);
                var endTime = DateTime.Now;
                var durationMs = (int)stopwatch.ElapsedMilliseconds;
                var logMessage = additionalDetails != null ? $"{errorMessage ?? ""} | Details: {additionalDetails}" : errorMessage;

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC [dbo].[spweb_InsertExecutionLog] @TaskName, @StartTime, @EndTime, @DurationMs, @Status, @ErrorMessage, @CreatedBy",
                    new SqlParameter("@TaskName", $"{actionType}: {actionName}"),
                    new SqlParameter("@StartTime", startTime),
                    new SqlParameter("@EndTime", endTime),
                    new SqlParameter("@DurationMs", durationMs),
                    new SqlParameter("@Status", status),
                    new SqlParameter("@ErrorMessage", logMessage ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedBy", createdBy)
                );

                throw;
            }

            if (status == "Success")
            {
                stopwatch.Stop();
                var durationMs = (int)stopwatch.ElapsedMilliseconds;
                _logger.LogInformation($"Action {actionName} ({actionType}) completed in {durationMs}ms.");
            }

            return result;
        }
    }

    //public async Task LogActionAsync(string actionName, string actionType, Func<Task> action, string createdBy = "System", string additionalDetails = null)
    //{
    //    //var startTime = DateTime.Now;
    //    var stopwatch = Stopwatch.StartNew();
    //    string status = "Success";
    //    string errorMessage = null;

    //    try
    //    {
    //        await action();
    //    }
    //    catch (Exception ex) when (!(ex is ValidationException))
    //    {
    //        // Log only non-validation exceptions
    //        status = "Failed";
    //        errorMessage = ex.ToString();
    //        _logger.LogError(ex, $"Action {actionName} ({actionType}) failed.");

    //        stopwatch.Stop();
    //        var startTime = DateTime.Now.AddMilliseconds(-stopwatch.ElapsedMilliseconds);
    //        var endTime = DateTime.Now;
    //        var durationMs = (int)stopwatch.ElapsedMilliseconds;
    //        var logMessage = additionalDetails != null ? $"{errorMessage ?? ""} | Details: {additionalDetails}" : errorMessage;

    //        await _context.Database.ExecuteSqlRawAsync(
    //            "EXEC [dbo].[spweb_InsertExecutionLog] @TaskName, @StartTime, @EndTime, @DurationMs, @Status, @ErrorMessage, @CreatedBy",
    //            new SqlParameter("@TaskName", $"{actionType}: {actionName}"),
    //            new SqlParameter("@StartTime", startTime),
    //            new SqlParameter("@EndTime", endTime),
    //            new SqlParameter("@DurationMs", durationMs),
    //            new SqlParameter("@Status", status),
    //            new SqlParameter("@ErrorMessage", logMessage ?? (object)DBNull.Value),
    //            new SqlParameter("@CreatedBy", createdBy)
    //        );

    //        throw;
    //    }
    //    //catch (Exception ex)
    //    //{
    //    //    status = "Failed";
    //    //    errorMessage = ex.Message;
    //    //    _logger.LogError(ex, $"Action {actionName} ({actionType}) failed.");
    //    //    throw;
    //    //}
    //    //finally
    //    //{
    //    //   // stopwatch.Stop();
    //    //    var endTime = DateTime.Now;
    //    //    var durationMs = (int)(int)(endTime - startTime).TotalMilliseconds;

    //    //    var logMessage = additionalDetails != null ? $"{errorMessage ?? ""} | Details: {additionalDetails}" : errorMessage;

    //    //    // Log to database
    //    //    await _context.Database.ExecuteSqlRawAsync(
    //    //        "EXEC [dbo].[spweb_InsertExecutionLog] @TaskName, @StartTime, @EndTime, @DurationMs, @Status, @ErrorMessage, @CreatedBy",
    //    //        new SqlParameter("@TaskName", $"{actionType}: {actionName}"),
    //    //        new SqlParameter("@StartTime", startTime),
    //    //        new SqlParameter("@EndTime", endTime),
    //    //        new SqlParameter("@DurationMs", durationMs),
    //    //        new SqlParameter("@Status", status),
    //    //        new SqlParameter("@ErrorMessage", logMessage ?? (object)DBNull.Value),
    //    //        new SqlParameter("@CreatedBy", createdBy)
    //    //    );

    //    //    // Log to application log
    //    //    if (status == "Success")
    //    //    {
    //    //        _logger.LogInformation($"Action {actionName} ({actionType}) completed in {durationMs}ms.");
    //    //    }
    //    //}
    //}
}