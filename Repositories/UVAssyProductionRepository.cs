using MESWebDev.Data;
using MESWebDev.Models.UVASSY;
using MESWebDev.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Repositories
{
    public class UVAssyProductionRepository : IUVAssyProductionRepository
    {
        private readonly AppDbContext _context;
        private readonly ILoggingService _loggingService;

        public UVAssyProductionRepository(AppDbContext context, ILoggingService loggingService)
        {
            _context = context;
            _loggingService = loggingService;
        }

        public async Task<List<UVAssyProduction>> GetProductionQuantitiesAsync()
        {
            var _productionQuantitiesSp = "uvmes_spweb_uvassy_output_error_qty";
            return await _loggingService.LogActionAsync<List<UVAssyProduction>>(
                actionName: "GetProductionQuantitiesAsync",
                actionType: "StoredProcedure",
                action: async () =>
                {
                    return await _context.UVAssyProductions
                        .FromSqlRaw($"EXEC {_productionQuantitiesSp}")
                        .AsNoTracking()
                        .ToListAsync();
                });
        }

        public async Task<List<UVAssyOutputDetail>> GetOutputDetailsAsync(string period)
        {
            var _outputDetailsSp = "uvmes_spweb_uvassy_output_detail";
            return await _loggingService.LogActionAsync<List<UVAssyOutputDetail>>(
                actionName: "GetOutputDetailsAsync",
                actionType: "StoredProcedure",
                action: async () =>
                {
                    return await _context.Set<UVAssyOutputDetail>()
                        .FromSqlRaw($"EXEC {_outputDetailsSp} @Period = @p0", new SqlParameter("@p0", period))
                        .AsNoTracking()
                        .ToListAsync();
                });
        }

        //public async Task<List<OutputDetail>> GetOutputDetailsAsync(string period)
        //{
        //    var _outputDetailsSp = "uvmes_spweb_uvassy_output_details"; // Hypothetical stored procedure
        //    return await _loggingService.LogActionAsync<List<OutputDetail>>(
        //        actionName: "GetOutputDetailsAsync",
        //        actionType: "StoredProcedure",
        //        action: async () =>
        //        {
        //            return await _context.Set<OutputDetail>()
        //                .FromSqlRaw($"EXEC {_outputDetailsSp} @Period = {period}")
        //                .AsNoTracking()
        //                .ToListAsync();
        //        });
        //}
        public async Task<List<UVAssyErrorDetail>> GetErrorDetailsAsync(string period)
        {
            var _errorDetailsSp = "uvmes_spweb_uvassy_error_detail";
            return await _loggingService.LogActionAsync<List<UVAssyErrorDetail>>(
                actionName: "GetErrorDetailsAsync",
                actionType: "StoredProcedure",
                action: async () =>
                {
                    return await _context.Set<UVAssyErrorDetail>()
                        .FromSqlRaw($"EXEC {_errorDetailsSp} @Period = @p0",
                            new SqlParameter("@p0", period))
                        .AsNoTracking()
                        .ToListAsync();
                });
        }

        //public async Task<List<ErrorDetail>> GetErrorDetailsAsync(string period)
        //{
        //    var _errorDetailsSp = "uvmes_spweb_uvassy_error_details"; // Hypothetical stored procedure
        //    return await _loggingService.LogActionAsync<List<ErrorDetail>>(
        //        actionName: "GetErrorDetailsAsync",
        //        actionType: "StoredProcedure",
        //        action: async () =>
        //        {
        //            return await _context.Set<ErrorDetail>()
        //                .FromSqlRaw($"EXEC {_errorDetailsSp} @Period = {period}")
        //                .AsNoTracking()
        //                .ToListAsync();
        //        });
        //}

        public async Task<List<UVAssyProductResult>> GetProductionResultsAsync(DateTime startDate, DateTime endDate)
        {
            var _productionResultsSp = "uvmes_spweb_uvassy_prod_result_realtime";
            return await _loggingService.LogActionAsync<List<UVAssyProductResult>>(
                actionName: "GetProductionResultsAsync",
                actionType: "StoredProcedure",
                action: async () =>
                {
                    return await _context.Set<UVAssyProductResult>()
                        .FromSqlRaw($"EXEC {_productionResultsSp} @StartDate = @p0, @EndDate = @p1",
                            new SqlParameter("@p0", startDate),
                            new SqlParameter("@p1", endDate))
                        .AsNoTracking()
                        .ToListAsync();
                });

            //var _productionResultsSp = "uvmes_spweb_uvassy_prod_result_realtime";
            //return await _loggingService.LogActionAsync<List<UVAssyProductResult>>(
            //    actionName: "GetProductionResultsAsync",
            //    actionType: "StoredProcedure",
            //    action: async () =>
            //    {
            //        return await _context.Set<UVAssyProductResult>()
            //            .FromSqlRaw($"EXEC {_productionResultsSp} @StartDate = @p0, @EndDate = @p1",
            //                new SqlParameter("@p0", startDate),
            //                new SqlParameter("@p1", endDate))
            //            .AsNoTracking()
            //            .ToListAsync();
            //    });
        }

        public async Task<List<UVAssyAllOutputResult>> GetAllOutputResultsAsync(DateTime date)
        {
            string formattedDate = date.ToString("yyyy/MM/dd");
            var _productionResultsSp = "uvmes_spweb_all_assy_output_realtime";
            return await _loggingService.LogActionAsync<List<UVAssyAllOutputResult>>(
                actionName: "GetProductionResultsAsync",
                actionType: "StoredProcedure",
                action: async () =>
                {
                    return await _context.Set<UVAssyAllOutputResult>()
                        .FromSqlRaw($"EXEC {_productionResultsSp} @CurrentDate = @p0",
                            new SqlParameter("@p0", formattedDate))
                        .AsNoTracking()
                        .ToListAsync();
                });
        }
    }
}