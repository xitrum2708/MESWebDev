using MESWebDev.DTO;
using MESWebDev.Repositories;
using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;
using System.Data;

namespace MESWebDev.Services
{
    public class RepairResultService : IRepairResultService
    {
        private readonly IRepairResultRepository _repo;
        private readonly string _connString;

        public RepairResultService(IRepairResultRepository repo, IConfiguration config)
        {
            _repo = repo;
            _connString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<List<RepairResultDto>> GetWithBulkCopyAsync(List<(string QRCode, string Partcode)> keys, DateTime? startDate, DateTime? endDate, string? searchText, string UserDept)
        {
            var raw = await _repo.GetFilteredAsync(startDate, endDate, searchText, UserDept);
            if (!raw.Any())
                return new List<RepairResultDto>();

            var dateCodeMap = new ConcurrentDictionary<(string, string), string>();
            var batchSize = 100;
            var maxDegreeOfParallelism = 4;

            try
            {
                var qrCodes = raw.Select(r => r.Qrcode).Distinct().ToList();
                var batches = qrCodes.Chunk(batchSize);

                await Parallel.ForEachAsync(batches, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism },
                    async (batch, ct) =>
                    {
                        using var conn = new SqlConnection(_connString);
                        await conn.OpenAsync(ct);
                        using var cmd = conn.CreateCommand();
                        cmd.CommandTimeout = 300;
                        cmd.CommandText = @"
                                            SELECT
                                                E.QRCode,
                                                E.Partcode,
                                                E.DateCode
                                            FROM EASTECH_SMT_OUTPUT E WITH (NOLOCK)
                                            WHERE E.QRCode IN (" + string.Join(",", batch.Select(q => $"'{q}'")) + ")";

                        using var reader = await cmd.ExecuteReaderAsync(ct);
                        while (await reader.ReadAsync(ct))
                        {
                            var qr = reader.GetString(0);
                            var part = reader.GetString(1);
                            var dateCode = reader.IsDBNull(2) ? null : reader.GetString(2);
                            var key = (qr, part);
                            dateCodeMap.AddOrUpdate(key, dateCode, (k, v) =>
                                string.IsNullOrEmpty(v) ? dateCode : $"{v};{dateCode}");
                        }
                    });

                var results = raw.Select(r => new RepairResultDto
                {
                    Qrcode = r.Qrcode,
                    Model = r.Model,
                    Lot = r.Lot,
                    DailyOutput = r.DailyOutput,
                    PcbCode = r.PcbCode,
                    Pcbtype = r.Pcbtype,
                    Process = r.Process,
                    Errorposition = r.Errorposition,
                    Partcode = r.Partcode,
                    Errortype = r.Errortype,
                    Causetype = r.Causetype,
                    DeptError = r.DeptError,
                    Repairmethod = r.Repairmethod,
                    Statusresult = r.Statusresult,
                    UserDept = r.UserDept,
                    Linename = r.Linename,
                    Qty = r.Qty,
                    CreatedDate = r.CreatedDate,
                    CreatedBy = r.CreatedBy,
                    Remark = r.Remark,
                    Soldermachine = r.Soldermachine,
                    Tinwire = r.Tinwire,
                    Flux = r.Flux,
                    Alcohol = r.Alcohol,
                    Other = r.Other,
                    DDRDate = r.DDRDate,
                    DDRKeyin = r.DDRKeyin,
                    DDRCHECK = r.DDRCHECK,
                    DDRDailyUpdate = r.DDRDailyUpdate,
                    DateCode = dateCodeMap.TryGetValue((r.Qrcode, r.Partcode), out var dc) ? dc : null
                }).ToList();

                return results;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching DateCodes", ex);
            }
        }
    }
}