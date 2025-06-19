using MESWebDev.Models.IQC;

namespace MESWebDev.Repositories
{
    public interface IIQCReportsRepository
    {
        Task<List<MonthlyRejectRateModel>> GetMonthlyRejectRateModelAsync(int year);

        Task<List<TopSupplierErrorReportModel>> GetMonthlyAcceptRateByGroupAsync(DateTime? startDate, DateTime? endDate);

        Task SaveItemAsync(UV_IQC_ReportItem reportItem);

        Task UpdateReportStatusAsync(UV_IQC_Report report);

        Task DeleteItemAsync(int reportItemId);
    }
}