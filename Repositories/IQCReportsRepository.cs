using MESWebDev.Data;
using MESWebDev.Models.IQC;
using MESWebDev.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Repositories
{
    public class IQCReportsRepository : IIQCReportsRepository
    {
        private readonly AppDbContext _context;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<IQCReportsRepository> _logger;

        public IQCReportsRepository(AppDbContext context, ILoggingService loggingService, ILogger<IQCReportsRepository> logger)
        {
            _context = context;
            _loggingService = loggingService;
            _logger = logger;
        }

        public async Task<List<MonthlyRejectRateModel>> GetMonthlyRejectRateModelAsync(int year)
        {
            var _productionResultsSp = "uvmes_spweb_IQCGetMonthlyRejectRateByGroup";
            return await _loggingService.LogActionAsync<List<MonthlyRejectRateModel>>(
                actionName: "GetMonthlyRejectRateModelAsync",
                actionType: "StoredProcedure",
                action: async () =>
                {
                    return await _context.Set<MonthlyRejectRateModel>()
                        .FromSqlRaw($"EXEC {_productionResultsSp} @Year = @p0",
                            new SqlParameter("@p0", year))
                        .AsNoTracking()
                        .ToListAsync();
                });
        }

        public async Task<List<TopSupplierErrorReportModel>> GetMonthlyAcceptRateByGroupAsync(DateTime? startDate, DateTime? endDate)
        {
            var _productionResultsSp = "uvmes_spweb_IQCGetMonthlyAcceptRateByGroup";
            return await _loggingService.LogActionAsync<List<TopSupplierErrorReportModel>>(
                actionName: "GetMonthlyAcceptRateByGroup",
                actionType: "StoredProcedure",
                action: async () =>
                {
                    return await _context.Set<TopSupplierErrorReportModel>()
                         .FromSqlRaw($"EXEC {_productionResultsSp} @StartDate = @p0, @EndDate = @p1",
                            new SqlParameter("@p0", startDate),
                            new SqlParameter("@p1", endDate))
                        .AsNoTracking()
                        .ToListAsync();
                });
        }

        public async Task SaveItemAsync(UV_IQC_ReportItem reportItem)
        {
            if (reportItem == null)
                throw new ArgumentNullException(nameof(reportItem));

            if (string.IsNullOrEmpty(reportItem.ReportID) || reportItem.ItemID == 0)
                throw new ArgumentException("ReportID and ItemID are required.", nameof(reportItem));

            try
            {
                _logger.LogInformation("Saving ReportItem: ReportID={ReportID}, ItemID={ItemID}", reportItem.ReportID, reportItem.ItemID);

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Entry(reportItem).State = EntityState.Detached;
                        // Check if exist then update
                        // If not then add new
                        var judgment = await _context.UV_IQC_ReportItems.Where(r => r.ReportID == reportItem.ReportID && r.ItemID == reportItem.ItemID && r.ItemName.Substring(0, 3) == reportItem.ItemName.Substring(0, 3)).FirstOrDefaultAsync();
                        if (judgment != null && (reportItem.ItemName.Substring(0, 3) == "E01" ||
                            reportItem.ItemName.Substring(0, 3) == "A01" ||
                            reportItem.ItemName.Substring(0, 3) == "R01" ||
                            reportItem.ItemName.Substring(0, 3) == "P01" ||
                            reportItem.ItemName.Substring(0, 3) == "E09" ||
                            reportItem.ItemName.Substring(0, 3) == "P09" ||
                            reportItem.ItemName.Substring(0, 3) == "A09" ||
                            reportItem.ItemName.Substring(0, 3) == "R09" ||
                            reportItem.ItemName.Substring(0, 3) == "E07" ||
                            reportItem.ItemName.Substring(0, 3) == "P07")
                            )
                        {
                            await _context.Database.ExecuteSqlRawAsync(
                            "UPDATE UV_IQC_ReportItems " +
                            "SET Judgment=@p13, " +
                            " CreatedBy=@p15, " +
                            " CreatedDate=@p16 " +
                            "WHERE ReportItemID=@p0",
                            new SqlParameter("@p0", judgment.ReportItemID),
                            new SqlParameter("@p13", reportItem.Judgment ?? (object)DBNull.Value),
                            new SqlParameter("@p15", reportItem.CreatedBy ?? (object)DBNull.Value),
                            new SqlParameter("@p16", reportItem.CreatedDate));

                            if (reportItem.Judgment == "ACCEPT" ||
                                reportItem.Judgment == "REJECT")
                            {
                                await _context.Database.ExecuteSqlRawAsync(
                                    "UPDATE [dbo].[UV_IQC_Reports] " +
                                    "SET CheckerStatus=@p13 " +
                                    "WHERE ReportID=@p0",
                                    new SqlParameter("@p0", reportItem.ReportID),
                                    new SqlParameter("@p13", reportItem.Judgment ?? (object)DBNull.Value));
                            }
                            transaction.Commit();
                            _logger.LogInformation("Successfully saved ReportItem: ReportID={ReportID}, ItemID={ItemID}", reportItem.ReportID, reportItem.ItemID);
                        }
                        else
                        {
                            await _context.Database.ExecuteSqlRawAsync(
                                "INSERT INTO UV_IQC_ReportItems (ReportID, ErrorCodeID, ItemID, ItemName, SamplingSize, Spec, SpecDetail, " +
                                "CRI, MAJ, MIN, NG_Total, NG_Rate, Standard, Judgment, Remark, CreatedBy, CreatedDate) " +
                                "VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16)",
                                new SqlParameter("@p0", reportItem.ReportID ?? (object)DBNull.Value),
                                new SqlParameter("@p1", reportItem.ErrorCodeID),
                                new SqlParameter("@p2", reportItem.ItemID),
                                new SqlParameter("@p3", reportItem.ItemName ?? (object)DBNull.Value),
                                new SqlParameter("@p4", reportItem.SamplingSize),
                                new SqlParameter("@p5", reportItem.Spec ?? (object)DBNull.Value),
                                new SqlParameter("@p6", reportItem.SpecDetail ?? (object)DBNull.Value),
                                new SqlParameter("@p7", reportItem.CRI),
                                new SqlParameter("@p8", reportItem.MAJ),
                                new SqlParameter("@p9", reportItem.MIN),
                                new SqlParameter("@p10", reportItem.NG_Total),
                                new SqlParameter("@p11", reportItem.NG_Rate),
                                new SqlParameter("@p12", reportItem.Standard ?? (object)DBNull.Value),
                                new SqlParameter("@p13", reportItem.Judgment ?? (object)DBNull.Value),
                                new SqlParameter("@p14", reportItem.Remark ?? (object)DBNull.Value),
                                new SqlParameter("@p15", reportItem.CreatedBy ?? (object)DBNull.Value),
                                new SqlParameter("@p16", reportItem.CreatedDate));

                            transaction.Commit();
                            _logger.LogInformation("Successfully saved ReportItem: ReportID={ReportID}, ItemID={ItemID}", reportItem.ReportID, reportItem.ItemID);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex, "Failed to save ReportItem: ReportID={ReportID}, ItemID={ItemID}", reportItem.ReportID, reportItem.ItemID);
                        throw new Exception($"Failed to save ReportItem: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save ReportItem: {ex.Message}", ex);
            }
        }

        public async Task DeleteItemAsync(int reportItemId)
        {
            var rec = _context.UV_IQC_ReportItems
                .Where(x => x.ReportItemID == reportItemId).FirstOrDefault();
            if (rec == null)
                throw new ArgumentException("ReportItemId is required.", nameof(reportItemId));

            try
            {
                _logger.LogInformation("Deleting ReportItem: ReportItemId={ReportItemId}", reportItemId);

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        int rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                            "DELETE FROM UV_IQC_ReportItems WHERE ReportItemID = @p0",
                            new SqlParameter("@p0", reportItemId));

                        if (rowsAffected == 0)
                        {
                            _logger.LogWarning("No ReportItem found with ReportItemId={ReportItemId}", reportItemId);
                            throw new Exception($"ReportItem with ID {reportItemId} not found.");
                        }

                        transaction.Commit();
                        _logger.LogInformation("Successfully deleted ReportItem: ReportItemId={ReportItemId}", reportItemId);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex, "Failed to delete ReportItem: ReportItemId={ReportItemId}", reportItemId);
                        throw new Exception($"Failed to delete ReportItem: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete ReportItem: {ex.Message}", ex);
            }
        }

        public async Task UpdateReportStatusAsync(UV_IQC_Report report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            if (string.IsNullOrEmpty(report.ReportID))
                throw new ArgumentException("ReportID are required.", nameof(report));
            try
            {
                _logger.LogInformation("Saving ReportItem: ReportID={ReportID}", report.ReportID);

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Entry(report).State = EntityState.Detached;
                        // Check if exist then update
                        // If not then add new

                        await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE UV_IQC_Reports " +
                        "SET Status=@p13, " +
                        "Notes =@p14, " +
                        "UpdatedDate =@p15, " +
                        "UpdatedBy =@p16, " +
                        "TextRemark=@p17 " +
                        "WHERE ReportID=@p0",
                        new SqlParameter("@p0", report.ReportID),
                        new SqlParameter("@p13", report.Status),
                        new SqlParameter("@p14", (object?)report.Notes ?? DBNull.Value),
                        new SqlParameter("@p15", report.UpdatedDate),
                        new SqlParameter("@p16", report.UpdatedBy ?? string.Empty),
                        new SqlParameter("@p17", (object?)report.TextRemark ?? DBNull.Value));

                        transaction.Commit();
                        _logger.LogInformation("Successfully saved ReportItem: ReportID={ReportID}, Status={Status}", report.ReportID, report.Status);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogInformation("Successfully saved ReportItem: ReportID={ReportID}, Status={Status}", report.ReportID, report.Status);
                        throw new Exception($"Failed to save ReportItem: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to Update Report: {ex.Message}", ex);
            }
        }
    }
}