using System.Data;

namespace MESWebDev.Common
{
    public interface IExcelExportService
    {
        byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName = "Sheet1");

        byte[] DatatableExportToExcel(DataTable data, string sheetName = "Sheet1");
    }
}