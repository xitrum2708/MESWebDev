using System.Data;

namespace MESWebDev.Services
{
    public interface IUVASSY_PPRODUCT_HISTORY
    {
        Task<DataTable> SearchResult(string search);
        Task<DataTable> GetAttachedList(int processID, string qrcode);
        Task<DataTable> GetAttachedFile(int Id);

    }
}
