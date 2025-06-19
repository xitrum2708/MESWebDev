using MESWebDev.Models.UVASSY;

namespace MESWebDev.Repositories
{
    public interface IUVAssyProductionRepository
    {
        Task<List<UVAssyProduction>> GetProductionQuantitiesAsync();

        Task<List<UVAssyOutputDetail>> GetOutputDetailsAsync(string period);

        Task<List<UVAssyErrorDetail>> GetErrorDetailsAsync(string period);

        Task<List<UVAssyProductResult>> GetProductionResultsAsync(DateTime startDate, DateTime endDate);

        Task<List<UVAssyAllOutputResult>> GetAllOutputResultsAsync(DateTime date);
    }
}