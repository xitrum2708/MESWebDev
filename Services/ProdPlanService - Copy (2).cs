using AutoMapper;
using ExcelDataReader;
using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Models.COMMON;
using MESWebDev.Models.ProdPlan;
using MESWebDev.Models.ProdPlan.PC;
using MESWebDev.Models.ProdPlan.SMT;
using MESWebDev.Models.ProdPlan.SMT.DTO;
using MESWebDev.Services.IService;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using static MESWebDev.Models.ProdPlan.SMT.DTO.FullCalendarDTO;

namespace MESWebDev.Services
{
    public class ProdPlanService3
    {
        private readonly AppDbContext _con;
        private readonly IMapper _mapper;
        private readonly Procedure _proc;
        private readonly IHttpContextAccessor _hca;
        private readonly ITranslationService _trans;
        private readonly DatatableListMap _dlm;

        public ProdPlanService3(AppDbContext con, IMapper mapper, IHttpContextAccessor hca, ITranslationService translation)
        {
            _con = con;
            _mapper = mapper;
            _proc = new Procedure(_con);
            _hca = hca;
            _trans = translation;
            _dlm = new();
        }


        #region PC Production Plan
        public async Task<ProdPlanViewModel> GetDataFromUploadFile(RequestDTO _request)
        {
            ProdPlanViewModel pp = new();
            List<IFormFile> files = _request.Files ?? new();
            if (files.Count == 0)
            {
                throw new ArgumentException("No files provided for processing.");
            }

            // GET ProdPlan Data
            try
            {
                ppm = new();
                pp.prod_plans = await GetProdData(files[0], files[1]);

                // GET Holiday Data
                pp.holidays = await GetHoliday(new());// GetHolidayData(files[2]);
                pp = await ProdPlanViewModel(pp);
            }
            catch (Exception ex)
            {
                string t = ex.Message;
            }

            return pp;
        }
        // GET random color for model
        private static readonly Random _rand = new Random();
        private string GetRandomColor()
        {
            var color = String.Format("#{0:X6}", _rand.Next(0x1000000));
            return color;
        }
        private string GetRandomRgbaColor(double alpha = 0.5)
        {
            int r = _rand.Next(256); // 0–255
            int g = _rand.Next(256);
            int b = _rand.Next(256);

            return $"rgba({r},{g},{b},{alpha.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
        }
        List<string> errors = new();
        public async Task<List<ProdPlanModel>> GetProdData(IFormFile file1, IFormFile file2)
        {
            List<ProdPlanModel> prodPlans = new();

            //--------------+ GET data from IFS +-----------------------------
            string csv = string.Empty;
            using (var reader = new StreamReader(file1.OpenReadStream(), Encoding.Default))
            {
                csv = await reader.ReadToEndAsync();
            }
            int has_header = 0;
            ProdPlanModel pp = new();
            if (csv != string.Empty)
            {
                foreach (var c in csv.Split('\n'))
                {

                    if (has_header > 0)
                    {
                        if (!string.IsNullOrEmpty(c.Trim()))
                        {
                            string t = c.Replace(@"\", "").Replace("\n", "").Replace("\r", "");
                            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                            //string[] q = t.Split("\t");
                            string[] q = t.Split(",");
                            try
                            {
                                pp = new ProdPlanModel()
                                {
                                    id = has_header
                                    ,
                                    model = q[0]
                                    ,
                                    lot_no = q[1]
                                    //,color = GetRandomColor()
                                    ,
                                    backgroundColor = GetRandomRgbaColor(0.5)
                                    ,
                                    borderColor = "transparent"
                                    ,
                                    lot_size = Convert.ToInt32(q[3])
                                    ,
                                    bal_qty = Convert.ToInt32(q[3])
                                };
                                prodPlans.Add(pp);

                            }
                            catch (Exception ex)
                            {
                                errors.Add(ex.Message);
                            }
                        }

                    }
                    has_header++;
                }
            }

            //--------------+ GET data from Master +-----------------------------
            using (var stream = new MemoryStream())
            {
                // Copy the IFormFile data to the memory stream
                file2.CopyTo(stream);
                stream.Position = 0; // Reset the stream position
                has_header = 0;
                // Read the Excel data
                try
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    //using (var reader = ExcelReaderFactory.CreateReader(stream, conf))
                    {

                        while (reader.Read()) // Each row of the file
                        {
                            has_header++;
                            if (has_header > 1 && reader.FieldCount > 1)// remove header
                            {
                                try
                                {
                                    string line = reader.GetValue(0).ToString();
                                    string model = reader.GetValue(1).ToString();
                                    string lot = reader.GetValue(2).ToString();

                                    bool is_new = string.IsNullOrEmpty(reader.GetValue(3)?.ToString())
                                                    || !reader.GetValue(3).ToString().Trim().Contains("1") ? false : true; // check if new model
                                    bool is_fpp = string.IsNullOrEmpty(reader.GetValue(4)?.ToString())
                                                    || !reader.GetValue(4).ToString().Trim().Contains("1") ? false : true; // check if FPP

                                    int capa_qty = Convert.ToInt32(reader.GetValue(5));
                                    var check = prodPlans.Where(i => i.model == model && i.lot_no == lot);
                                    if (check.Any())
                                    {
                                        foreach (var c in check)
                                        {
                                            c.line = line;
                                            c.capa_qty = capa_qty;
                                            c.is_new = is_new;
                                            c.is_fpp = is_fpp;
                                        }
                                    }
                                    else
                                    {
                                        errors.Add($"Model {model} and Lot no {lot} does not exist in master data !");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errors.Add($"{ex.Message}");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{ex.Message}");
                }
            }
            return prodPlans;
        }

        public async Task<List<string>> GetHolidayData(IFormFile file3)
        {
            //List<CalendarModel> calendar = new();
            List<string> holiday = new();


            //--------------+ GET data from Master +-----------------------------
            using (var stream = new MemoryStream())
            {
                // Copy the IFormFile data to the memory stream
                file3.CopyTo(stream);
                stream.Position = 0; // Reset the stream position
                int has_header = 0;

                // Read the Excel data
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                //using (var reader = ExcelReaderFactory.CreateReader(stream, conf))
                {

                    while (reader.Read()) // Each row of the file
                    {
                        has_header++;
                        if (has_header > 1 && reader.FieldCount > 1)// remove header
                        {
                            try
                            {
                                int year = Convert.ToInt32(reader.GetValue(0));
                                int month = Convert.ToInt32(reader.GetValue(1));
                                string dates = Convert.ToString(reader.GetValue(2) ?? "");
                                if (!string.IsNullOrEmpty(dates))
                                {
                                    foreach (var date in dates.Split('/'))
                                    {
                                        if (!string.IsNullOrEmpty(date))
                                        {
                                            DateTime h = new DateTime(year, month, Convert.ToInt32(date));
                                            holiday.Add(h.ToString("yyyy-MM-dd"));
                                        }

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"{ex.Message}");
                            }
                        }
                    }
                }
            }
            return holiday;
        }

        private DateTime GetDate(DateTime date, List<string> holidays)
        {
            var check = holidays.Where(h => Convert.ToDateTime(h).Date == date.Date);
            if (check.Any())
            {
                // If the date is a holiday, return the next day
                return GetDate(date.AddDays(1), holidays);
            }
            else
            {
                // If not a holiday, return the date
                return date;
            }
        }


        // Calculate Data, default is 8 hours/ days
        List<ProdPlanModel> ppm = new();
        public async Task<ProdPlanViewModel> CalProdPlan(ProdPlanViewModel ppv)
        {

            //List<ProdPlanModel> ppm = new();
            List<ProdPlanModel> _ppm = ppv.prod_plans ?? new();

            List<string> lines = new();
            if (_ppm != null && _ppm.Any())
                lines = _ppm.GroupBy(i => i.line).Select(i => i.Key).ToList();

            DateTime sch_dt = ppv.sch_dt ?? DateTime.Now.Date;
            foreach (var line in lines)
            {
                ppv.line = line;
                ppv.sch_dt = sch_dt;
                ppv.start_hour = 8;
                ppv.is_other_model = false;
                ppv.is_first_run_fpp_model = true;
                ppv.is_first_run_new_model = true;
                ppv.start_sch_dt = ppv.start_sch_dt ?? DateTime.Now.Date;
                foreach (var p in _ppm.Where(i => i.line == line).ToList())
                {
                    ppv.prodPlanModel = p;
                    await _CalProdPlan(ppv);
                }
            }
            ppv.prod_plans = ppm;
            return ppv;
        }

        public async Task<ProdPlanViewModel> _CalProdPlan(ProdPlanViewModel ppv)  //!!!!!!!!!! thay đổi model + 30 phút
        {
            const int DefaultStartHour = 8;
            const int BreakHour = 12;
            const int BreakDuration = 1;
            const int DefaultNewModelRate = 65;

            ProdPlanModel p = ppv.prodPlanModel;
            DateTime start_sch_date = Convert.ToDateTime(ppv.start_sch_dt);
            int new_model_rate = ppv.new_model_rate ?? DefaultNewModelRate;

            DateTime dt = GetDate(ppv.sch_dt ?? DateTime.Now, ppv.holidays).Date;
            DateTime start_break = dt.AddHours(ppv.break_working_time ?? BreakHour);
            DateTime start_run = dt.AddHours(ppv.start_hour ?? DefaultStartHour);
            int start_working_time = ppv.start_working_time ?? DefaultStartHour;
            int working_hour = ppv.working_hour ?? 8;
            DateTime endOfWorking = dt.AddHours(start_working_time + working_hour + 1); // 1 hour for break

            bool is_other_model = ppm.Count > 0 && ppm.Last().model != (p?.model ?? "") ? ppv.is_other_model ?? false : false;
            int model_transfer_time = ppv.model_stransfer_time ?? 30;

            // --- First Run for FPP ---
            if (p?.is_fpp == true && ppv.is_first_run_fpp_model == true && p.lot_size == p.bal_qty)
            {
                if ((ppv.start_hour ?? 0) == 8 && is_other_model)
                {
                    ppm.Add(CreateProdPlan(p, start_run, endOfWorking, working_hour, ppv.line, start_sch_date, Math.Min(p.capa_qty, p.bal_qty)));
                    ppv.sch_dt = start_run.Date.AddDays(1);
                    ppv.start_hour = DefaultStartHour;
                    ppv.prodPlanModel.bal_qty -= p.qty;
                    ppv.is_other_model = p.bal_qty <= p.capa_qty;
                    ppv.is_first_run_fpp_model = false;

                    if (p.bal_qty > p.capa_qty)
                        return await _CalProdPlan(ppv);
                    return ppv;
                }

                ppv.sch_dt = ppm.Count == 0 ? start_run : start_run.AddDays(1);
                ppv.start_hour = DefaultStartHour;
                ppv.is_other_model = true;

                return await _CalProdPlan(ppv);
            }

            // --- Continue Scheduling ---
            if (is_other_model)
                start_run = start_run.AddMinutes(model_transfer_time);

            if (start_run >= start_break && start_run < start_break.AddHours(BreakDuration))
                start_run = start_run.AddHours(BreakDuration);

            if (start_run >= endOfWorking)
            {
                ppv.sch_dt = start_run.AddDays(1);
                ppv.start_hour = DefaultStartHour + (start_run - endOfWorking).TotalHours;
                ppv.is_other_model = false;
                return await _CalProdPlan(ppv);
            }

            try
            {
                double finish = ((double)p.bal_qty * 8) / (double)p.capa_qty;
                double minus = start_run >= start_break ? 0 : 1;

                int qty = (int)Math.Round(((endOfWorking - start_run).TotalHours - minus) / 8 * p.capa_qty);
                if (p.is_new && ppv.is_first_run_new_model == true)
                {
                    finish = finish * 100 / new_model_rate;
                    ppv.is_first_run_new_model = false;
                    qty = (int)Math.Round(((((endOfWorking - start_run).TotalHours - minus) / 8 * p.capa_qty) * new_model_rate) / 100);
                }

                DateTime finish_run = start_run.AddHours(finish);

                if (start_run < start_break && finish_run > start_break)
                    finish_run = finish_run.AddHours(BreakDuration);

                if (finish_run > endOfWorking)
                {
                    ppm.Add(CreateProdPlan(p, start_run, endOfWorking, working_hour, ppv.line, start_sch_date, qty));
                    ppv.sch_dt = start_run.Date.AddDays(1);
                    ppv.start_hour = DefaultStartHour;
                    ppv.prodPlanModel.bal_qty -= qty;
                    ppv.is_other_model = false;
                    return await _CalProdPlan(ppv);
                }

                // Final Case
                ppm.Add(CreateProdPlan(p, start_run, finish_run, working_hour, ppv.line, start_sch_date, p.bal_qty));
                ppv.sch_dt = start_run.Date;
                ppv.start_hour = (finish_run - start_run.Date).TotalHours;
                ppv.is_other_model = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Or log properly
            }

            return ppv;
        }

        // -- Helper --
        ProdPlanModel CreateProdPlan(ProdPlanModel p, DateTime start, DateTime end, int workingHour, string line, DateTime startSchDate, int? qty = null)
        {
            return new ProdPlanModel
            {
                line = line,
                id = p.id,
                working_hour = workingHour,
                qty = qty ?? p.qty,
                start = start,
                end = end,
                model = p.model,
                lot_no = p.lot_no,
                lot_size = p.lot_size,
                capa_qty = p.capa_qty,
                bal_qty = p.bal_qty,
                backgroundColor = p.backgroundColor,
                borderColor = p.borderColor,
                is_fpp = p.is_fpp,
                is_new = p.is_new,
                start_sch_dt = startSchDate
            };
        }


        public async Task<ProdPlanViewModel> ReloadProdPlan(ProdPlanViewModel ppv)
        {
            try
            {

                // Step 1: Identify changes (line or start time)
                var changeLineDict = ppv.events
                    .Where(i => i.resourceId != i.line || Convert.ToDateTime(i.start).AddHours(7).ToString("yyyy-MM-dd HH:mm:ss") != Convert.ToDateTime(i.old_start).ToString("yyyy-MM-dd HH:mm:ss"))
                    .GroupBy(i => i.id)
                    .ToDictionary(
                        g => g.Key,
                        g => new EventsDTO
                        {
                            id = g.Key,
                            line = g.First().resourceId,
                            start = g.Min(j => j.start)
                        }
                    );

                // Step 2: Build main event list
                var ev = ppv.events
                    .GroupBy(i => new
                    {
                        i.id,
                        i.model,
                        i.lot_no,
                        i.capa_qty,
                        i.lot_size,
                        i.is_fpp,
                        i.is_new,
                        i.backgroundColor,
                        i.borderColor
                    })
                    .Select(g =>
                    {
                        var first = g.First();
                        return new EventsDTO
                        {
                            id = g.Key.id,
                            line = first.resourceId,
                            model = g.Key.model,
                            lot_no = g.Key.lot_no,
                            capa_qty = g.Key.capa_qty,
                            bal_qty = g.Max(k => k.bal_qty),
                            start = g.Min(k => k.start),
                            lot_size = g.Key.lot_size,
                            is_fpp = g.Key.is_fpp,
                            is_new = g.Key.is_new,
                            backgroundColor = g.Key.backgroundColor,
                            borderColor = g.Key.borderColor
                        };
                    })
                    .Select(e =>
                    {
                        if (changeLineDict.TryGetValue(e.id, out var change))
                        {
                            e.line = change.line;
                            e.start = change.start;
                        }
                        return e;
                    })
                    .ToList();


                List<ProdPlanModel> prodPlans = _mapper.Map<List<ProdPlanModel>>(ev);
                ppm = new();
                ppv.prod_plans = prodPlans.OrderBy(i => i.line).ThenBy(i => i.start).ToList();
                ppv = await ProdPlanViewModel(ppv);

            }
            catch (Exception ex)
            {
                return ppv;
            }

            return ppv;
        }

        // return ProdPlanViewModel
        public async Task<ProdPlanViewModel> ProdPlanViewModel(ProdPlanViewModel ppv)
        {
            ppv = await CalProdPlan(ppv);

            ppv.prod_plans = ppm;

            if (ppm.Any())
            {
                ppv.resources = ppm.GroupBy(i => i.line).Select(i => new ResourcesDTO
                {
                    id = i.Key,
                    title = i.Key
                }).ToList();


                ppv.events = _mapper.Map<List<EventsDTO>>(ppm);
                ppv.events.ForEach(i =>
                {
                    i.resourceId = i.line;
                    i.old_start = i.start;
                }); // set resourceId for each event
                ppv.line_items = ppv.resources
                    .Select(i => new SelectListItem
                    {
                        Value = i.id,
                        Text = i.title
                    }).ToList();

            }
            return ppv;
        }

        public async Task<List<string>> GetHolidays(RequestDTO _request = null)
        {
            var holidays = _con.PP_Calendar_tbl.Where(i => i.is_holiday == true
                                            && (_request == null || _request == new RequestDTO() || (i.date.Year == _request.year && i.date.Month == _request.month)));

            if (holidays.Any())
                return holidays.Select(i => i.date.ToString("yyyy-MM-dd")).ToList();
            else return new();
        }

        public async Task<ProdPlanViewModel> ViewProdPlan(RequestDTO _request)
        {
            ProdPlanViewModel ppv = new();
            try
            {
                // Get all production plans
                var prodPlans = _con.PP_ProdPlan_tbl;
                if (prodPlans.Any())
                {
                    DateTime date = prodPlans.OrderByDescending(i => i.start_sch_dt).First().start_sch_dt;
                    ppv.start_sch_dt = date;
                    ppv.prod_plans = prodPlans.Where(i => i.start_sch_dt == date).ToList();

                    ppv.holidays = await GetHolidays();

                    ppv.resources = new();
                    ppv.events = new();
                    if (ppv.prod_plans.Any())
                    {
                        ppv.resources = ppv.prod_plans.GroupBy(i => i.line).Select(i => new ResourcesDTO
                        {
                            id = i.Key,
                            title = i.Key
                        }).ToList();

                        ppv.events = _mapper.Map<List<EventsDTO>>(ppv.prod_plans);
                        ppv.events.ForEach(i =>
                        {
                            i.resourceId = i.line;
                            i.old_start = i.start;
                            i.id = i.old_id;
                        }); // set resourceId for each event
                        ppv.line_items = ppv.resources
                            .Select(i => new SelectListItem
                            {
                                Value = i.id,
                                Text = i.title
                            }).ToList();
                        //ppv.events = ppv.events.OrderBy(i => i.line).ThenBy(i => i.id).ToList();
                    }
                    Dictionary<string, object> para = await GetParas();
                    // model_change_time
                    if (para.TryGetValue("model_change_time", out var value))
                    {
                        ppv.model_stransfer_time = Convert.ToInt32(value);
                    }
                    // model_fisrt_run_percent
                    if (para.TryGetValue("model_fisrt_run_percent", out var value2))
                    {
                        ppv.new_model_rate = Convert.ToInt32(value2);
                    }

                }
                else
                {
                    ppv.prod_plans = new();
                    ppv.holidays = new();
                    ppv.resources = new();
                    ppv.events = new();
                    ppv.start_sch_dt = null;
                    ppv.line_items = new();
                }
                // Get all holidays
            }
            catch (Exception ex)
            {
                string t = ex.Message;
            }
            return ppv;
        }

        public async Task<string> SaveProdPlan(ProdPlanViewModel ppv)
        {
            string msg = string.Empty;
            List<ProdPlanModel> prodPlans = ppv.prod_plans ?? new List<ProdPlanModel>();
            if (prodPlans.Count == 0)
            {
                msg = "No production plan data to save.";
                return msg;
            }

            // This will avoid the case: delete --> timeout --> not add data yet
            using var transaction = await _con.Database.BeginTransactionAsync();

            try
            {
                // Step 1: Delete existing data
                var targetDate = Convert.ToDateTime(ppv.start_sch_dt).Date;
                // Step 1: Delete all records (if needed)
                await _con.Database.ExecuteSqlRawAsync(@"DELETE FROM PP_ProdPlan_tbl WHERE CAST(start_sch_dt AS DATE) = {0};", targetDate);
                try
                {
                    var maxId = await _con.PP_ProdPlan_tbl.MaxAsync(p => (int?)p.id) ?? 0;
                    maxId = maxId == 0 ? 0 : maxId + 1;
                    await _con.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT ('PP_ProdPlan_tbl', RESEED, {maxId})");
                }
                catch { }

                // Step 2: Prepare new data
                DateTime now = DateTime.Now;
                prodPlans.ForEach(i =>
                {
                    i.old_id = i.id; // Store old id for later use
                    i.start_sch_dt = now;
                    i.created_by = ""; // Set actual user if needed
                    i.created_date = DateTime.Now;
                    i.id = 0;
                });

                // Step 3: Insert new data
                await _con.PP_ProdPlan_tbl.AddRangeAsync(prodPlans);
                await _con.SaveChangesAsync();

                // Step 4: Commit transaction
                await transaction.CommitAsync();

                msg = "Production plan saved successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                msg = "Error saving production plan: " + ex.Message;
            }

            try
            {

                if (ppv.holidays == null || !ppv.holidays.Any())
                {
                    return msg;
                }
                await addHolidays(ppv.holidays);

            }
            catch (Exception ex)
            {
                msg += $" Error saving holidays: {ex.Message}";
            }
            return msg;
        }

        public async Task<DataSet> ExportProdPlan(Dictionary<string, object> dic)
        {
            DataSet ds = new();
            DataSet dset = await _proc.Proc_GetDataset("PP_ProdPlan_Export", dic);
            if (dset != null && dset.Tables.Count > 1)
            {
                DataTable data = new();// dset.Tables[0];
                DataTable holiday = new();// dset.Tables[1];
                if (dset.Tables[0] != null && dset.Tables[0].Rows.Count > 0)
                {
                    if (dset.Tables[1].Rows.Count > 0)
                    {
                        holiday = dset.Tables[1].Copy();
                    }
                    holiday.TableName = "holiday";
                    ds.Tables.Add(holiday);

                    // remove the first row that is holiday
                    data = dset.Tables[0].Copy();
                    data.TableName = "COMB";
                    data.Rows.RemoveAt(0);
                    ds.Tables.Add(data);

                    var line_list = data.AsEnumerable()
                        .Select(row => row.Field<string>("line"))
                        .Distinct()
                        .ToList();
                    foreach (var line in line_list)
                    {
                        DataTable dt = data.Clone();
                        dt.TableName = line;
                        var rows = data.AsEnumerable().Where(r => r.Field<string>("line") == line).CopyToDataTable();
                        foreach (DataRow row in rows.Rows)
                        {
                            dt.ImportRow(row);
                        }
                        ds.Tables.Add(dt);
                    }

                }
            }
            return ds;
        }

        public async Task<List<string>> SaveHoliday(List<string> holidays)
        {
            await addHolidays(holidays);
            return await GetHolidays();
        }

        public async Task addHolidays(List<string> holidays)
        {
            //string msg = string.Empty;
            List<CalendarModel> calendar = new();
            calendar = holidays?.Select(i => new CalendarModel
            {
                date = Convert.ToDateTime(i),
                is_holiday = true,
                note = "Holiday"
            }).ToList() ?? new List<CalendarModel>();
            // Step 1: Delete all records (if needed)
            await _con.Database.ExecuteSqlRawAsync(@"DELETE FROM PP_Calendar_tbl WHERE is_holiday = 1");
            //try
            //{
            //    var maxId = await _con.PP_ProdPlan_tbl.MaxAsync(p => (int?)p.id) ?? 0;
            //    maxId = maxId == 0 ? 0 : maxId + 1;
            //    await _con.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT ('PP_Calendar_tbl', RESEED, 0)");
            //}
            //catch { }

            await _con.AddRangeAsync(calendar);
            await _con.SaveChangesAsync();
            //return msg;
        }

        public async Task<List<string>> GetHoliday(RequestDTO _request)
        {
            return await GetHolidays(_request);
        }

        public async Task<string> SavePara(ProdPlanViewModel ppv)
        {
            string msg = "Update data successfully";
            try
            {
                var data = _con.PP_Para_tbl.Where(i => i.name == "model_change_time");
                if (data.Any())
                {
                    data.First().value = ppv.model_stransfer_time?.ToString() ?? "30";
                }
                var data2 = _con.PP_Para_tbl.Where(i => i.name == "model_fisrt_run_percent");
                if (data2.Any())
                {
                    data2.First().value = ppv.new_model_rate?.ToString() ?? "65";
                }
                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        public async Task<Dictionary<string, object>> GetPara()
        {
            return await GetParas();
        }

        public async Task<Dictionary<string, object>> GetParas()
        {
            Dictionary<string, object> dic = new();

            var data = _con.PP_Para_tbl;
            if (data.Any())
                dic = await data.ToDictionaryAsync(i => i.name, i => (object)i.value);
            return dic;
        }

        #endregion

        #region SMT Production Plan

        #region SMT Lot PCB Management
        public async Task<DataTable> SMTLotPcbList(Dictionary<string, object> dic)
        {
            DataTable dt = await _proc.Proc_GetDatatable("spweb_UV_SMT_ProdPlan_LotPcb", dic);
            return dt;
        }

        public async Task<SMTLotPcbModel> SMTLotPcbDetail(int Id)
        {
            return await _con.UV_SMT_Lot_PCB.FindAsync(Id) ?? new();
        }

        public async Task<string> SMTLotPcbAdd(SMTLotPcbModel slp)
        {
            try
            {
                var check = await _con.UV_SMT_Lot_PCB.FirstOrDefaultAsync(i => i.Lotno == slp.Lotno && i.Model == slp.Model);
                if (check != null)
                {
                    return "Duplicate Lot No. and Model";
                }
                slp.CreatedDt = DateTime.Now;
                slp.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system";
                await _con.UV_SMT_Lot_PCB.AddAsync(slp);
                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        public async Task<ProdPlanViewModel> SMTLotPcbUpload(IFormFile file)
        {
            ProdPlanViewModel ppv = new();
            try
            {
                ppv = await GetSMTLotPcbUploadData(file);
                List<SMTLotPcbModel> lst = ppv.SMTLotPcbList ?? new();
                if (lst.Count > 0)
                {
                    string LotnoString = string.Join(", ", lst.Select(i => i.Lotno));

                    // delete all existing data
                    await _con.UV_SMT_Lot_PCB
                                            .Where(i => LotnoString.Contains(i.Lotno))
                                            //.ForEachAsync(i => _con.UV_SMT_Lot_PCB.Remove(i));
                                            .ExecuteDeleteAsync();
                    await _con.SaveChangesAsync();
                    // add new one
                    await _con.UV_SMT_Lot_PCB.AddRangeAsync(lst);
                    await _con.SaveChangesAsync();
                    ppv.Data = await SMTLotPcbList(new Dictionary<string, object>()
                    {
                        { "@upload_file", ppv.UploadedFile}
                    });
                }
                else ppv.error_msg = SD.ErrorMsg.NoDataInUploadedFile;
            }
            catch (Exception ex)
            {
                ppv.error_msg = ex.Message;
            }

            return ppv;
        }

        public async Task<ProdPlanViewModel> GetSMTLotPcbUploadData(IFormFile file)
        {
            ProdPlanViewModel ppv = new();
            List<SMTLotPcbModel> lst = new();
            string uploadedFile = $"{file.FileName} {DateTime.Now:yyMMddHHmmss}";
            DateTime dt = DateTime.Now;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    List<UploadFileMaster> ufm = await _con.Master_UploadFile_mst
                                                        .Where(i => i.file_name.ToUpper().Contains("PCB"))
                                                        .ToListAsync();
                    Dictionary<string, int> colMap = ufm.ToDictionary(i => i.db_col_name, i => i.col_index);
                    int header_row = ufm.FirstOrDefault()?.header_row ?? 0;
                    int count = 0;
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Read())
                        {
                            count++;
                            if (count <= header_row) continue; // skip header

                            string pcbNo = CommonFormat.GetValueOrDefault(reader, colMap, "PcbNo", string.Empty);
                            string pcb = CommonFormat.GetValueOrDefault(reader, colMap, "PcbNo", string.Empty);
                            pcbNo = pcbNo.Length >= 6
                                            ? pcbNo.Substring(1, 2) + pcbNo.Substring(4, 6)
                                            : pcbNo;

                            string model = CommonFormat.GetValueOrDefault(reader, colMap, "Model", string.Empty);
                            model = model.Length >= 6
                                            ? model.Substring(0, 6)
                                            : model;
                            string lotno = CommonFormat.GetValueOrDefault(reader, colMap, "LotNo", string.Empty);
                            if (!lotno.Contains("FCST") && (model.StartsWith("UY2") || model.StartsWith("UY3") || !model.StartsWith("UY")))
                            {
                                SMTLotPcbModel slp = new()
                                {
                                    Lotno = lotno,
                                    PCBNo = pcbNo,
                                    PCB = pcb,
                                    Model = model,
                                    UploadedFile = uploadedFile,
                                    //Remark = "Data from IFS System",
                                    CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system",
                                    CreatedDt = dt
                                };
                                lst.Add(slp);
                            }
                            if (lst.Count > 0)
                            {
                                lst = lst.DistinctBy(i => new { i.Model, i.Lotno, i.PCBNo }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ppv.error_msg = ex.Message;
            }

            ppv.SMTLotPcbList = lst;
            ppv.UploadedFile = uploadedFile;
            return ppv;
        }

        public async Task<string> SMTLotPcbEdit(SMTLotPcbModel slp)
        {

            var check = await _con.UV_SMT_Lot_PCB.FindAsync(slp.Id);
            if (check == null)
            {
                return SD.ErrorMsg.NotExisted;
            }
            check.Model = slp.Model;
            check.PCBNo = slp.PCBNo;
            check.PCB = slp.PCB;
            check.Lotno = slp.Lotno;
            check.Remark = slp.Remark;
            check.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system";
            check.UpdatedDt = DateTime.Now;
            await _con.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> SMTLotPcbDelete(List<int> Ids)
        {
            string msg = string.Empty;
            try
            {
                foreach (var item in Ids)
                {
                    var check = await _con.UV_SMT_Lot_PCB.FindAsync(item);
                    _con.UV_SMT_Lot_PCB.Remove(check);
                }
                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }

        #endregion

        #region UV Plan
        public async Task<DataTable> UVPlanList(Dictionary<string, object> dic)
        {
            DataTable dt = await _proc.Proc_GetDatatable("spweb_UV_SMT_ProdPlan_UVPlan", dic);
            return dt;
        }

        public async Task<SMTPlanModel> UVPlanDetail(int Id)
        {
            return await _con.UV_SMT_Plan.FindAsync(Id) ?? new();
        }

        public async Task<string> UVPlanAdd(SMTPlanModel slp)
        {
            try
            {
                var check = await _con.UV_SMT_Plan.FirstOrDefaultAsync(i => i.Lotno == slp.Lotno && i.Model == slp.Model);
                if (check != null)
                {
                    return "Duplicate Lot No. and Model";
                }
                slp.CreatedDt = DateTime.Now;
                slp.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system";
                await _con.UV_SMT_Plan.AddAsync(slp);
                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        public async Task<ProdPlanViewModel> UVPlanUpload(IFormFile file)
        {
            ProdPlanViewModel ppv = new();
            try
            {
                ppv = await GetUVPlanUploadData(file);
                List<SMTPlanModel> lst = ppv.SMTPlanList ?? new();
                if (lst.Count > 0)
                {
                    string LotnoString = string.Join(", ", lst.Select(i => i.Lotno));

                    // delete all existing data
                    await _con.UV_SMT_Plan
                                            .Where(i => LotnoString.Contains(i.Lotno))
                                            //.ForEachAsync(i => _con.UV_SMT_Plan.Remove(i));
                                            .ExecuteDeleteAsync();
                    await _con.SaveChangesAsync();
                    // add new one
                    await _con.UV_SMT_Plan.AddRangeAsync(lst);
                    await _con.SaveChangesAsync();
                    ppv.Data = await UVPlanList(new Dictionary<string, object>()
                    {
                        { "@upload_file", ppv.UploadedFile}
                    });
                }
                else ppv.error_msg = SD.ErrorMsg.NoDataInUploadedFile;
            }
            catch (Exception ex)
            {
                ppv.error_msg = ex.Message;
            }

            return ppv;
        }

        public async Task<ProdPlanViewModel> GetUVPlanUploadData(IFormFile file)
        {
            ProdPlanViewModel ppv = new();
            List<SMTPlanModel> lst = new();
            string uploadedFile = $"{file.FileName} {DateTime.Now:yyMMddHHmmss}";
            DateTime dt = DateTime.Now;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    List<UploadFileMaster> ufm = await _con.Master_UploadFile_mst
                                                        .Where(i => i.file_name.ToUpper().Contains("UVPlan"))
                                                        .ToListAsync();
                    Dictionary<string, int> colMap = ufm.ToDictionary(i => i.db_col_name, i => i.col_index);
                    int header_row = ufm.FirstOrDefault()?.header_row ?? 0;
                    int count = 0;
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Read())
                        {
                            count++;
                            if (count <= header_row) continue; // skip header
                            string model = CommonFormat.GetValueOrDefault(reader, colMap, "Model", string.Empty);
                            if (!string.IsNullOrEmpty(model) && !model.ToLower().Contains("adjust") && !model.StartsWith("%"))
                            {
                                DateTime defaultDt = new DateTime(1900, 1, 1);
                                string lotno = CommonFormat.GetValueOrDefault(reader, colMap, "LotNo", string.Empty);
                                DateTime startDt = CommonFormat.GetValueOrDefault(reader, colMap, "StartDt", defaultDt);
                                DateTime endDt = CommonFormat.GetValueOrDefault(reader, colMap, "EndDt", defaultDt);
                                int size = CommonFormat.GetValueOrDefault(reader, colMap, "Size", 0);
                                int balance = CommonFormat.GetValueOrDefault(reader, colMap, "Balance", 0);
                                if (startDt == defaultDt || endDt == defaultDt || string.IsNullOrEmpty(lotno) || balance == 0) continue; // skip if no date

                                SMTPlanModel slp = new()
                                {
                                    StartDt = startDt,
                                    EndDt = endDt,
                                    Lotno = lotno,
                                    Size = size,
                                    Balance = balance,
                                    //PCBNo = pcbNo,
                                    Model = model,
                                    UploadedFile = uploadedFile,
                                    Remark = file.FileName,
                                    CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system",
                                    CreatedDt = dt
                                };
                                lst.Add(slp);
                            }
                            if (lst.Count > 0)
                            {
                                lst = lst.DistinctBy(i => new { i.Model, i.Lotno }).ToList();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ppv.error_msg = ex.Message;
            }

            ppv.SMTPlanList = lst;
            ppv.UploadedFile = uploadedFile;
            return ppv;
        }

        public async Task<string> UVPlanEdit(SMTPlanModel slp)
        {
            var check = await _con.UV_SMT_Plan.FindAsync(slp.Id);
            if (check == null)
            {
                return SD.ErrorMsg.NotExisted;
            }
            check.Size = slp.Size;
            check.Balance = slp.Balance;
            check.StartDt = slp.StartDt;
            check.EndDt = slp.EndDt;
            check.UpdatedDt = DateTime.Now;
            check.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system";
            check.Remark = check.Remark;
            await _con.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> UVPlanDelete(List<int> Ids)
        {
            string msg = string.Empty;
            try
            {
                foreach (var item in Ids)
                {
                    var check = await _con.UV_SMT_Plan.FindAsync(item);
                    _con.UV_SMT_Plan.Remove(check);
                }
                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }

        #endregion

        #region SMT Production Plan
        public async Task<DataTable> SMTPlanTrackingList(Dictionary<string, object> dic)
        {
            DataTable dt = await _proc.Proc_GetDatatable("spweb_UV_SMT_PlanTracking", dic);
            return dt;
        }

        public async Task<SMTProdPlanModel> SMTPlanTrackingDetail(int Id)
        {
            return await _con.UV_SMT_Prod_Plan.FindAsync(Id) ?? new();
        }

        public async Task<string> SMTPlanTrackingAdd(SMTProdPlanModel slp)
        {
            try
            {
                var check = await _con.UV_SMT_Prod_Plan.FirstOrDefaultAsync(i => i.Lotno == slp.Lotno 
                                                                            && i.Model == slp.Model 
                                                                            && i.PCBNo == slp.PCBNo);
                if (check != null)
                {
                    return "Duplicate Lot No. and Model and PcbNo.";
                }
                slp.CreatedDt = DateTime.Now;
                slp.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system";
                await _con.UV_SMT_Prod_Plan.AddAsync(slp);
                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        public async Task<ProdPlanViewModel> SMTPlanTrackingUpload(IFormFile file)
        {
            ProdPlanViewModel ppv = new();
            try
            {
                ppv = await GetSMTPlanTrackingUploadData(file);
                List<SMTProdPlanModel> lst = ppv.SMTProdPlanList ?? new();
                if (lst.Count > 0)
                {
                    string LotnoString = string.Join(", ", lst.Select(i => i.PCBKey));

                    // delete all existing data
                    await _con.UV_SMT_Prod_Plan
                                            .Where(i => LotnoString.Contains(i.PCBKey))
                                            //.ForEachAsync(i => _con.UV_SMT_Prod_Plan.Remove(i));
                                            .ExecuteDeleteAsync();
                    await _con.SaveChangesAsync();
                    // add new one
                    await _con.UV_SMT_Prod_Plan.AddRangeAsync(lst);
                    await _con.SaveChangesAsync();
                    ppv.Data = await SMTPlanTrackingList(new Dictionary<string, object>()
                    {
                        { "@upload_file", ppv.UploadedFile}
                    });
                }
                else ppv.error_msg = SD.ErrorMsg.NoDataInUploadedFile;
            }
            catch (Exception ex)
            {
                ppv.error_msg = ex.Message;
            }

            return ppv;
        }

        public async Task<ProdPlanViewModel> GetSMTPlanTrackingUploadData(IFormFile file)
        {
            ProdPlanViewModel ppv = new();
            List<SMTProdPlanModel> lst = new();
            string uploadedFile = $"{file.FileName} {DateTime.Now:yyMMddHHmmss}";
            DateTime dt = DateTime.Now;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    List<UploadFileMaster> ufm = await _con.Master_UploadFile_mst
                                                        .Where(i => i.file_name.ToUpper().Contains("SMTProdPlan"))
                                                        .ToListAsync();
                    Dictionary<string, int> colMap = ufm.ToDictionary(i => i.db_col_name, i => i.col_index);
                    int header_row = ufm.FirstOrDefault()?.header_row ?? 0;
                    int count = 0;
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                count++;
                                if (count <= header_row) continue; // skip header
                                string model = CommonFormat.GetValueOrDefault(reader, colMap, "Model", string.Empty);
                                if (!string.IsNullOrEmpty(model) && model.ToUpper().StartsWith("U")
                                    && (model.StartsWith("UY2") || model.StartsWith("UY3") || !model.StartsWith("UY"))
                                    && !model.ToLower().Contains("adjust") && !model.StartsWith("%"))
                                {
                                    DateTime defaultDt = new DateTime(1900, 1, 1);
                                    DateTime FinishedDt = CommonFormat.GetValueOrDefault(reader, colMap, "FinishedDt", defaultDt);

                                    string LineCode = CommonFormat.GetValueOrDefault(reader, colMap, "LineCode", string.Empty);
                                    string Market = CommonFormat.GetValueOrDefault(reader, colMap, "Market", string.Empty);
                                    string Model = CommonFormat.GetValueOrDefault(reader, colMap, "Model", string.Empty);
                                    string PCBKey = CommonFormat.GetValueOrDefault(reader, colMap, "PCBKey", string.Empty);
                                    string Lotno = CommonFormat.GetValueOrDefault(reader, colMap, "Lotno", string.Empty);
                                    string PCBType = CommonFormat.GetValueOrDefault(reader, colMap, "PCBType", string.Empty);
                                    string PCBNo = CommonFormat.GetValueOrDefault(reader, colMap, "PCBNo", string.Empty);
                                    string MachineCode = CommonFormat.GetValueOrDefault(reader, colMap, "MachineCode", string.Empty);
                                    string KeyCode = CommonFormat.GetValueOrDefault(reader, colMap, "KeyCode", string.Empty);
                                    int PCBPerModel = CommonFormat.GetValueOrDefault(reader, colMap, "PCBPerModel", 0);
                                    int LotSize = CommonFormat.GetValueOrDefault(reader, colMap, "LotSize", 0);
                                    int IssuedQty = CommonFormat.GetValueOrDefault(reader, colMap, "IssuedQty", 0);
                                    int BalanceQty = CommonFormat.GetValueOrDefault(reader, colMap, "BalanceQty", 0);
                                    int TargetPerHour85 = CommonFormat.GetValueOrDefault(reader, colMap, "TargetPerHour85", 0);
                                    decimal TargetPerShift = CommonFormat.GetValueOrDefault(reader, colMap, "TargetPerShift", 0m);
                                    decimal TimeF = CommonFormat.GetValueOrDefault(reader, colMap, "TimeF", 0m);
                                    string Remark = CommonFormat.GetValueOrDefault(reader, colMap, "Remark", string.Empty);
                                    string Warning = CommonFormat.GetValueOrDefault(reader, colMap, "Warning", string.Empty);
                                    string UVNote = CommonFormat.GetValueOrDefault(reader, colMap, "UVNote", string.Empty);
                                    string ETPCB = CommonFormat.GetValueOrDefault(reader, colMap, "ETPCB", string.Empty);


                                    if (string.IsNullOrEmpty(Lotno) || string.IsNullOrEmpty(Model) || string.IsNullOrEmpty(PCBNo)) continue; // skip if no date

                                    SMTProdPlanModel slp = new()
                                    {
                                        FinishedDt = FinishedDt,
                                        LineCode = LineCode,
                                        Market = Market,
                                        Model = Model,
                                        PCBKey = PCBKey,
                                        Lotno = Lotno,
                                        PCBType = PCBType,
                                        PCBNo = PCBNo,
                                        MachineCode = MachineCode,
                                        KeyCode = KeyCode,
                                        PCBPerModel = PCBPerModel,
                                        LotSize = LotSize,
                                        IssuedQty = LotSize - BalanceQty > 0 ? LotSize - BalanceQty : 0,//IssuedQty,
                                        BalanceQty = BalanceQty,
                                        TargetPerHour85 = TargetPerHour85,
                                        TargetPerShift = TargetPerShift,
                                        TimeF = TimeF,
                                        Remark = Remark,
                                        Warning = Warning,
                                        UVNote = UVNote,
                                        ETPCB = ETPCB,
                                        IsFinished = BalanceQty > 0 ? false : true,

                                        UploadedFile = uploadedFile,
                                        CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system",
                                        CreatedDt = dt
                                    };
                                    lst.Add(slp);
                                }
                            }
                            catch
                            {

                            }
                            
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ppv.error_msg = ex.Message;
            }

            ppv.SMTProdPlanList = lst;
            ppv.UploadedFile = uploadedFile;
            return ppv;
        }

        public async Task<string> SMTPlanTrackingEdit(SMTProdPlanModel slp)
        {
            var check = await _con.UV_SMT_Prod_Plan.FindAsync(slp.Id);
            if (check == null)
            {
                return SD.ErrorMsg.NotExisted;
            }
            check.FinishedDt = slp.FinishedDt;
            check.IssuedQty = slp.IssuedQty;
            check.BalanceQty = slp.BalanceQty;
            check.TargetPerHour85 = slp.TargetPerHour85;
            check.Remark = slp.Remark;
            check.Warning = slp.Warning;
            check.ExcessStock = slp.ExcessStock;
            check.UpdatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system";
            check.UpdatedDt = DateTime.Now;

            await _con.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> SMTPlanTrackingDelete(List<int> Ids)
        {
            string msg = string.Empty;
            try
            {
                foreach (var item in Ids)
                {
                    var check = await _con.UV_SMT_Prod_Plan.FindAsync(item);
                    _con.UV_SMT_Prod_Plan.Remove(check);
                }
                await _con.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }
        #endregion

        #endregion

        #region Master

        //-------- Machine --------//
        #region ------------- Machine --------------
        public async Task<DataTable> MachineList(Dictionary<string, object> dic)
        {
            //spweb_UV_SMT_Master_Machine
            DataTable dt = await _proc.Proc_GetDatatable("spweb_UV_SMT_Master_Machine", dic);
            return dt;
        }

        public async Task<SMTMachineModel> MachineDetail(string machineId)
        {
            var data = await _con.UV_SMT_Mst_Machine.FindAsync(machineId);
            return data ?? new();
        }

        public async Task<string> MachineAdd(SMTMachineModel machine)
        {
            var data = await _con.UV_SMT_Mst_Machine.FindAsync(machine.MachineCode);
            if (data == null)
            {
                await _con.UV_SMT_Mst_Machine.AddAsync(machine);
                await _con.SaveChangesAsync();
                return string.Empty;
            }
            return machine.MachineCode;
        }

        public async Task<string> MachineEdit(SMTMachineModel machine)
        {
            var data = await _con.UV_SMT_Mst_Machine.FindAsync(machine.MachineCode);
            if (data == null) return machine.MachineCode;

            data.MachineName = machine.MachineName;
            data.Remark = machine.Remark;
            data.IsActive = machine.IsActive;
            await _con.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> MachineDelete(string machineId)
        {
            var data = await _con.UV_SMT_Mst_Machine.FindAsync(machineId);
            if (data == null) return machineId;

            _con.Remove(data);
            await _con.SaveChangesAsync();
            return string.Empty;
        }
        #endregion

        //-------- Line --------//
        #region ------------ Line --------------
        public async Task<DataTable> LineList(Dictionary<string, object> dic)
        {
            DataTable dt = await _proc.Proc_GetDatatable("spweb_UV_SMT_Master_Line", dic);
            return dt;
        }

        public async Task<SMTLineModel> LineDetail(string lineId)
        {
            var data = await _con.UV_SMT_Mst_Line.FindAsync(lineId);
            return data ?? new();
        }

        public async Task<string> LineAdd(SMTLineModel line)
        {
            var data = await _con.UV_SMT_Mst_Line.FindAsync(line.LineCode);
            if (data == null)
            {
                await _con.UV_SMT_Mst_Line.AddAsync(line);
                await _con.SaveChangesAsync();
                return string.Empty;
            }
            return line.LineCode;
        }

        public async Task<string> LineEdit(SMTLineModel line)
        {
            var data = await _con.UV_SMT_Mst_Line.FindAsync(line.LineCode);
            if (data == null) return line.LineCode;
            data.LineName = line.LineName;
            data.DisplayOrder = line.DisplayOrder;
            data.DisplayColor = line.DisplayColor;
            data.Remark = line.Remark;
            data.IsActive = line.IsActive;
            await _con.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> LineDelete(string lineId)
        {
            var data = await _con.UV_SMT_Mst_Line.FindAsync(lineId);
            if (data == null) return lineId;
            _con.UV_SMT_Mst_Line.Remove(data);
            await _con.SaveChangesAsync();
            return string.Empty;
        }
        #endregion

        //-------- Machine Condition --------//
        #region -------------- Machine Condition ---------------
        public async Task<DataTable> MachineConditionList(Dictionary<string, object> dic)
        {
            DataTable dt = await _proc.Proc_GetDatatable("spweb_UV_SMT_Master_MachineCondition", dic);
            return dt;
        }

        public async Task<SMTMachineConditionModel> MachineConditionDetail(int Id)
        {
            var data = await _con.UV_SMT_Mst_MachineCondition.Include(i => i.Machine).FirstOrDefaultAsync(i=>i.Id == Id);
            return data ?? new();
        }

        public async Task<string> MachineConditionAdd(SMTMachineConditionModel mc)
        {
            var data = await _con.UV_SMT_Mst_MachineCondition
                                    .FirstOrDefaultAsync(i=>i.MachineCode == mc.MachineCode && i.ChipMin == mc.ChipMin && i.ChipMax == mc.ChipMax);
            try
            {
                if (data != null)
                {
                    _con.UV_SMT_Mst_MachineCondition.Remove(data);
                    await _con.SaveChangesAsync();
                }
                await _con.UV_SMT_Mst_MachineCondition.AddAsync(mc);
                await _con.SaveChangesAsync();
                return string.Empty;
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }

        public async Task<string> MachineConditionEdit(SMTMachineConditionModel mc)
        {
            var data = await _con.UV_SMT_Mst_MachineCondition.FindAsync(mc.Id);

            if (data == null) return $"{mc.Id.ToString()} {_trans.Trans(SD.ErrorMsg.NotExisted)}";

            var check = _con.UV_SMT_Mst_MachineCondition
                                    .Where(i => i.MachineCode == mc.MachineCode && i.ChipMin == mc.ChipMin
                                                                                && i.ChipMax == mc.ChipMax && i.Id != mc.Id);
            if (check.Any()) return _trans.Trans(SD.ErrorMsg.Duplicated);
            data.ChipMin = mc.ChipMin;
            data.ChipMax = mc.ChipMax;
            data.Remark = mc.Remark;
            data.Priority = mc.Priority;            

            await _con.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> MachineConditionDelete(int Id)
        {
            var data = await _con.UV_SMT_Mst_MachineCondition.FindAsync(Id);
            if (data == null) return $"{Id.ToString()} {_trans.Trans(SD.ErrorMsg.NotExisted)}";
            _con.UV_SMT_Mst_MachineCondition.Remove(data);
            await _con.SaveChangesAsync();
            return string.Empty;
        }

        #endregion

        //-------- Line Machine --------//
        #region -------------- Line Machine ---------------

        public async Task<DataTable> LineMachineList(Dictionary<string, object> dic)
        {
            DataTable dt = await _proc.Proc_GetDatatable("spweb_UV_SMT_Master_LineMachine", dic);
            return dt;
        }
    
        public async Task<SMTLineMachineDataModel> LineMachineDetail(int Id)
        {
            var data = await _con.UV_SMT_LineMachineData.Include(i=>i.Line).Include(i => i.Machine).FirstOrDefaultAsync(i => i.Id == Id);
            return data ?? new();
        }

        public async Task<string> LineMachineAdd(SMTLineMachineDataModel line)
        {
            var data = await _con.UV_SMT_LineMachineData.FirstOrDefaultAsync(i=> i.LineCode == line.LineCode && i.MachineCode == line.MachineCode);
            if (data == null)
            {
                line.UsageDate = DateTime.Now;
                line.CreatedBy = _hca.HttpContext.User.Identity.Name ?? "System";
                await _con.UV_SMT_LineMachineData.AddAsync(line);
                await _con.SaveChangesAsync();
                return string.Empty;
            }
            return $"Line {line.LineCode} - {line.MachineCode} {SD.ErrorMsg.Existed}";
        }

        public async Task<string> LineMachineEdit(SMTLineMachineDataModel line)
        {
            var check = await _con.UV_SMT_LineMachineData.FirstOrDefaultAsync(i => i.LineCode == line.LineCode && i.MachineCode == line.MachineCode && i.Id != line.Id);

            if (check != null) return $"Line {line.LineCode} - {line.MachineCode} {_trans.Trans(SD.ErrorMsg.Existed)}";
            var data = await _con.UV_SMT_LineMachineData.FindAsync(line.Id);
            if (data == null) return $"ID: {line.Id.ToString()} {_trans.Trans(SD.ErrorMsg.Existed)}";

            data.LineCode = line.LineCode;
            data.MachineCode = line.MachineCode;
            data.Remark = line.Remark;
            data.UsagePercent = line.UsagePercent;
            data.UsageDate =  DateTime.Now;
            data.CreatedBy = _hca.HttpContext.User.Identity.Name ?? "System";
            await _con.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> LineMachineDelete(int Id)
        {
            var data = await _con.UV_SMT_LineMachineData.FindAsync(Id);
            if (data == null) return $"{Id.ToString()} {_trans.Trans(SD.ErrorMsg.NotExisted)}";
            _con.UV_SMT_LineMachineData.Remove(data);
            await _con.SaveChangesAsync();
            return string.Empty;
        }

        #endregion

        //-------- Shift Master --------//
        #region -------------- Shift Master ---------------
        public async Task<DataTable> ShiftList(Dictionary<string, object> dic)
        {
            DataTable dt = await _proc.Proc_GetDatatable("spweb_UV_SMT_Master_Shift", dic);
            return dt;
        }

        public async Task<SMTShiftModel> ShiftDetail(string shiftCode)
        {
            var data = await _con.UV_SMT_Mst_Shift.FindAsync(shiftCode);
            return data ?? new();
        }

        public async Task<string> ShiftAdd(SMTShiftModel shift)
        {
            var check = await _con.UV_SMT_Mst_Shift.FindAsync(shift.ShiftCode); 
            if(check == null)
            {
                await _con.UV_SMT_Mst_Shift.AddAsync(shift);
                await _con.SaveChangesAsync();

                List<SMTShiftDtlModel> lstDtl = await ParseShiftTime(shift.Pattern, shift.ShiftCode);
                if (lstDtl.Count > 0)
                {
                    await _con.UV_SMT_Mst_Shift_Dtl.AddRangeAsync(lstDtl);
                    await _con.SaveChangesAsync();
                }
                return string.Empty;
            }
            return $"{shift.ShiftCode} {_trans.Trans(SD.ErrorMsg.Existed)}";
        }

        public async Task<string> ShiftEdit(SMTShiftModel shift)
        {
            var data = await _con.UV_SMT_Mst_Shift.FindAsync(shift.ShiftCode);
            if (data == null) return $"{shift.ShiftCode} {_trans.Trans(SD.ErrorMsg.NotExisted)}";
            data.ShiftName = shift.ShiftName;
            data.Priority = shift.Priority;
            data.Remark = shift.Remark;
            data.Pattern = shift.Pattern;

            List<SMTShiftDtlModel> lstDtl = await ParseShiftTime(shift.Pattern, shift.ShiftCode);
            if (lstDtl.Count > 0)
            {
                var existingDtl = _con.UV_SMT_Mst_Shift_Dtl.Where(i => i.ShiftCode == shift.ShiftCode);
                _con.UV_SMT_Mst_Shift_Dtl.RemoveRange(existingDtl);
                await _con.UV_SMT_Mst_Shift_Dtl.AddRangeAsync(lstDtl);
                await _con.SaveChangesAsync();
            }

            await _con.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> ShiftDelete(string shiftCode)
        {
            var data = await _con.UV_SMT_Mst_Shift.FindAsync(shiftCode);
            if (data == null) return $"{shiftCode} {_trans.Trans(SD.ErrorMsg.NotExisted)}";
            _con.UV_SMT_Mst_Shift.Remove(data);
            var existingDtl = _con.UV_SMT_Mst_Shift_Dtl.Where(i => i.ShiftCode == shiftCode);
            _con.UV_SMT_Mst_Shift_Dtl.RemoveRange(existingDtl);
            await _con.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<List<SelectListItem>> SLILine()
        {
            var data = await _con.UV_SMT_Mst_Line
                                .Where(i => i.IsActive == true)
                                .OrderBy(i => i.DisplayOrder)
                                .ToListAsync();
            return data.Select(i => new SelectListItem { Text = i.LineCode, Value = i.LineCode }).ToList();   
        }

        public async Task<List<SelectListItem>> SLIMachine()
        {
            var data = await _con.UV_SMT_Mst_Machine
                                .Where(i => i.IsActive == true)
                                .OrderBy(i => i.MachineCode)
                                .ToListAsync();
            return data.Select(i => new SelectListItem { Text = i.MachineCode, Value = i.MachineCode }).ToList();
        }

        public async Task<List<SelectListItem>> SLIShift()
        {
            var data = await _con.UV_SMT_Mst_Shift
                                .OrderBy(i => i.Priority)
                                .ToListAsync();
            return data.Select(i => new SelectListItem { Text = i.ShiftCode, Value = i.ShiftCode }).ToList();
        }
        public async Task<List<SMTShiftDtlModel>> ParseShiftTime(string timeArrange, string shiftCode)
        {
            List<SMTShiftDtlModel> lst = new();
            if (string.IsNullOrEmpty(timeArrange))
            {
                lst.Add(new SMTShiftDtlModel()
                {
                    ShiftCode = shiftCode
                    ,
                    StartTime = string.Empty
                    ,
                    EndTime = string.Empty
                    ,
                    StartMinute = 0
                    ,
                    EndMinute = 0
                });
            }
            else
            {
                string[] parts = timeArrange.Split(';', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return lst;
                foreach (string part in parts)
                {
                    string[] range = part.Split('-', StringSplitOptions.RemoveEmptyEntries);
                    if (range.Length == 2)
                    {
                        int startMinute = await ToMinute(range[0]);
                        int endMinute = await ToMinute(range[1]);
                        if (startMinute < endMinute) {
                            lst.Add(new SMTShiftDtlModel()
                            {
                                ShiftCode = shiftCode
                                ,
                                StartTime = range[0]
                                ,
                                EndTime = range[1]
                                ,
                                StartMinute = startMinute
                                ,
                                EndMinute = endMinute
                            });
                        }
                    }
                }
            }            
            return lst;
        }

        public async Task<int> ToMinute(string timeStr)
        {
            int totalMinutes = 0;
            try
            {
                string[] parts = timeStr.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    int hours = int.Parse(parts[0]);
                    int minutes = int.Parse(parts[1]);
                    totalMinutes = hours * 60 + minutes;
                }
            }
            catch { }
            return totalMinutes;
        }
        #endregion

        //-------- Shift Master --------//
        #region --------------Line Calender Master ---------------
        public async Task<DataTable> LineCalendarList(Dictionary<string, object> dic)
        {
            DataTable dt = await _proc.Proc_GetDatatable("spweb_UV_SMT_Master_LineCalendar", dic);
            return dt;
        }

        public async Task<SMTLineCalendarModel> LineCalendarDetail(int Id)
        {
            var data = await _con.UV_SMT_Mst_LineCalendar.FindAsync(Id);
            return data ?? new();
        }

        public async Task<string> LineCalendarAdd(SMTLineCalendarModel Calendar)
        {
            var check = await _con.UV_SMT_Mst_LineCalendar.FindAsync(Calendar.Id);
            if (check == null)
            {
                await _con.UV_SMT_Mst_LineCalendar.AddAsync(Calendar);
                await _con.SaveChangesAsync();
                
                List<SMTLineCalendarDtlModel> lstDtl = await ParseCalendar(Calendar.WeekDayOrDate, Calendar.Id);
                if (lstDtl.Count > 0)
                {
                    await _con.UV_SMT_Mst_LineCalendar_Dtl.AddRangeAsync(lstDtl);
                    await _con.SaveChangesAsync();
                }

                return string.Empty;
            }
            return $"{Calendar.Id.ToString()} {_trans.Trans(SD.ErrorMsg.Existed)}";
        }

        public async Task<string> LineCalendarEdit(SMTLineCalendarModel Calendar)
        {
            var data = await _con.UV_SMT_Mst_LineCalendar.FindAsync(Calendar.Id);
            if (data == null) return $"{Calendar.Id.ToString()} {_trans.Trans(SD.ErrorMsg.NotExisted)}";

            data.LineCode = Calendar.LineCode;
            data.WeekDayOrDate = Calendar.WeekDayOrDate;
            data.ShiftCode = Calendar.ShiftCode;
            data.Priority = Calendar.Priority;
            data.Remark = Calendar.Remark;
            data.IsActive = Calendar.IsActive;

            List<SMTLineCalendarDtlModel> lstDtl = await ParseCalendar(Calendar.WeekDayOrDate, Calendar.Id);
            if (lstDtl.Count > 0)
            {
                var existingDtl = _con.UV_SMT_Mst_LineCalendar_Dtl.Where(i => i.HeaderId == Calendar.Id);
                _con.UV_SMT_Mst_LineCalendar_Dtl.RemoveRange(existingDtl);

                await _con.UV_SMT_Mst_LineCalendar_Dtl.AddRangeAsync(lstDtl);
                await _con.SaveChangesAsync();
            }

            await _con.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> LineCalendarDelete(int Id)
        {
            var data = await _con.UV_SMT_Mst_LineCalendar.FindAsync(Id);
            if (data == null) return $"{Id.ToString()} {_trans.Trans(SD.ErrorMsg.NotExisted)}";

            _con.UV_SMT_Mst_LineCalendar.Remove(data);

            var existingDtl = _con.UV_SMT_Mst_LineCalendar_Dtl.Where(i => i.HeaderId == Id);
            _con.UV_SMT_Mst_LineCalendar_Dtl.RemoveRange(existingDtl);

            await _con.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<List<SMTLineCalendarDtlModel>> ParseCalendar(string calendar, int headerId)
        {
            List<SMTLineCalendarDtlModel> lst = new();
            string[] parts = calendar.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return lst;
            foreach (string part in parts) {
                if (part.Any(char.IsLetter))
                {
                    if (part.Contains("-"))
                    {
                        string[] range = part.Split('-', StringSplitOptions.RemoveEmptyEntries);
                        if (range.Length == 2)
                        {
                            int start = Array.IndexOf(SD.Weekdays, range[0]);
                            int end = Array.IndexOf(SD.Weekdays, range[1]);
                            for(int i = start; i <= end; i++)
                            {
                                lst.Add(new SMTLineCalendarDtlModel()
                                {
                                    HeaderId = headerId
                                    ,
                                    ConditionType = SD.CalendarType.Weekday
                                    ,
                                    WeekDay = SD.Weekdays[i]
                                });
                            }
                        }
                    }
                    else
                    {
                        lst.Add(new SMTLineCalendarDtlModel()
                        {
                            HeaderId = headerId
                            ,
                            ConditionType = SD.CalendarType.Weekday
                            ,
                            WeekDay = part
                        });
                    }
                }
                else if (part.Contains("-"))
                {
                    string[] range = part.Split('-', StringSplitOptions.RemoveEmptyEntries);
                    if (range.Length == 2)
                    {
                        lst.Add(new SMTLineCalendarDtlModel()
                        {
                            HeaderId = headerId
                            ,
                            ConditionType = SD.CalendarType.Range
                            ,
                            StartDate = Convert.ToDateTime(range[0])
                            ,
                            EndDate = Convert.ToDateTime(range[1])
                        });
                    }

                }
                else
                {
                    lst.Add(new SMTLineCalendarDtlModel()
                    {
                        HeaderId = headerId
                        ,
                        ConditionType = SD.CalendarType.Date
                        ,
                        StartDate = Convert.ToDateTime(part)
                    });
                }

            }
            return lst;
        }


        public async Task<ProdPlanViewModel> IniSMTProdPlan(DateTime start, DateTime end)
        {
            ProdPlanViewModel pr = new();

            DataSet ds = await _proc.Proc_GetDataset("spweb_UV_SMT_ProdPlan_GeneratePreview_CO", new Dictionary<string, object>()
            {
                //{ "@StartDate", start },
                //{ "@EndDate", end },
                { "@Debug", 0}
            });

            if (ds.Tables.Count < 2)
            {
                return pr;
            }
            DataTable data = ds.Tables[ds.Tables.Count - 1];

            List<SMTProdPlanDTO> lstPlan = new();
            lstPlan = _dlm.ConvertToList<SMTProdPlanDTO>(data);// DataTableToList<SMTProdPlanDTO>(data);

            pr.SMTProdPlanDTOList = lstPlan;
            pr = await SMTProdPlanViewModel(pr);            
            return pr;
        }

        public async Task<PreviewResponse> IniSMTProdPlan2(DateTime start, DateTime end)
        {
            PreviewResponse pr = new();
            var resMap = new Dictionary<string, FcResource>(StringComparer.OrdinalIgnoreCase);
            var events = new List<FcEvent>();

            DataSet ds= await _proc.Proc_GetDataset("spweb_UV_SMT_ProdPlan_GeneratePreview_CO", new Dictionary<string, object>()
            {
                //{ "@StartDate", start },
                //{ "@EndDate", end },
                { "@Debug", 0}
            });

            if (ds.Tables.Count < 2)
            {
                return pr;
            }

            DataTable data = ds.Tables[ds.Tables.Count-1];

            foreach (DataRow dr in data.Rows) {
                var line = dr["LineCode"]?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (!resMap.ContainsKey(line))
                    resMap[line] = new FcResource(line, line);

                var lotno = dr["Lotno"]?.ToString() ?? "";
                var pcbKey = dr["PCBKey"]?.ToString() ?? "";
                var ro = Convert.ToInt32(dr["RunOrder"]);
                var qty = Convert.ToInt32(dr["Qty"]);
                var startDt = (DateTime)dr["StartDt"];
                var endDt = (DateTime)dr["EndDt"];

                var machine = dr["MachineCode"]?.ToString() ?? "";
                var model = dr["Model"]?.ToString() ?? "";
                var pcbNo = dr["PCBNo"]?.ToString() ?? "";
                var pcbType = dr["PCBType"]?.ToString() ?? "";

                events.Add(new FcEvent
                {
                    id = $"{lotno}-{pcbKey}-{startDt:yyyyMMddHHmmss}",
                    resourceId = line,
                    title = $"{lotno} | {pcbKey} | RO{ro} | {qty}",
                    start = startDt.ToString("s"),
                    end = endDt.ToString("s"),
                    allDay = false,
                    extendedProps = new
                    {
                        Lotno = lotno,
                        PCBKey = pcbKey,
                        RunOrder = ro,
                        Qty = qty,
                        LineCode = line,
                        MachineCode = machine,
                        Model = model,
                        PCBNo = pcbNo,
                        PCBType = pcbType
                    }
                });
            }

            var resources = resMap.Values.OrderBy(r => r.id).ToList();

            // Break background: common draw from Shift_Dtl (only create base on range requested)
            var background = await BuildBreakBackgroundEvents(resources.Select(r => r.id).ToList(), start.Date, end.Date);

            pr.resources = resources;
            pr.events = events;
            pr.backgroundEvents = background;
            return pr;
        }

        // draw break: get working segments from UV_SMT_Mst_Shift_Dtl,
        // after that get "gaps" (0..1440) => break events
        private async Task<List<FcEvent>> BuildBreakBackgroundEvents(
            List<string> lineCodes,
            DateTime startDate,
            DateTime endDate)
        {
            // If range is too big ( eg: year), payload break will be easy to overload.
            // Common draw: Limit break <= 62 day(s), if over, remove break.
            var totalDays = (endDate.Date - startDate.Date).TotalDays;
            if (totalDays > 62) return new List<FcEvent>();
            var segs = _con.UV_SMT_Mst_Shift_Dtl
                                .Select(s => new { s.StartMinute, s.EndMinute })
                                .ToList()
                                .Select(x => (startMin: x.StartMinute, endMin: x.EndMinute))
                                .ToList();

            // merge working intervals
            segs = segs.OrderBy(x => x.startMin).ToList();
            var merged = new List<(int s, int e)>();

            /*-------------------------
             * merged.Count == 0 means no interval yet → add first one.
             * merged[^1] means the last element in merged
             * (^1 is C# index-from-end syntax)
             * 
             * ------------------------*/
            foreach (var it in segs)
            {
                if (merged.Count == 0 || it.startMin > merged[^1].e)
                    merged.Add((it.startMin, it.endMin));
                else
                    merged[^1] = (merged[^1].s, Math.Max(merged[^1].e, it.endMin));
            }

            // breaks = gaps between merged working intervals
            var breaks = new List<(int s, int e)>();
            int cur = 0;
            foreach (var w in merged)
            {
                if (w.s > cur) breaks.Add((cur, w.s));
                cur = w.e;
            }
            if (cur < 1440) breaks.Add((cur, 1440));

            var result = new List<FcEvent>();

            for (var day = startDate.Date; day < endDate.Date; day = day.AddDays(1))
            {
                foreach (var line in lineCodes)
                {
                    foreach (var br in breaks)
                    {
                        // end exclusive in FullCalendar semantics; safe to use next day midnight
                        var st = day.AddMinutes(br.s);
                        var en = day.AddMinutes(br.e);

                        result.Add(new FcEvent
                        {
                            id = $"break-{line}-{day:yyyyMMdd}-{br.s}-{br.e}",
                            resourceId = line,
                            start = st.ToString("s"),
                            end = en.ToString("s"),
                            display = "background",
                            backgroundColor = "#f2f2f2",
                            overlap = false
                        });
                    }
                }
            }

            return result;
        }


        //----------------- SMT Production Plan -----------------//

        public async Task<ProdPlanViewModel> SMTViewProdPlan(RequestDTO _request)
        {
            ProdPlanViewModel ppv = new();
            try
            {
                // Get all production plans
                var prodPlans = _con.UV_SMT_Prod_Plan_Dtl.Include(i=>i.SMTProdPlanHdrModel);
                if (prodPlans.Any())
                {
                    DateTime date = prodPlans.OrderByDescending(i => i.StartScheduleDate).First().StartScheduleDate;
                    ppv.start_sch_dt = date;
                    ppv.SMTProdPlanDtlList = prodPlans.Where(i => i.StartScheduleDate == date).ToList();

                    ppv.holidays = await GetHolidays();

                    ppv.resources = new();
                    ppv.events = new();
                    if (ppv.SMTProdPlanDtlList.Any())
                    {
                        ppv.resources = ppv.SMTProdPlanDtlList.GroupBy(i => i.LineCode).Select(i => new ResourcesDTO
                        {
                            id = i.Key,
                            title = i.Key
                        }).ToList();

                        ppv.SMTEventsList = _mapper.Map<List<SMTEventsDTO>>(ppv.SMTProdPlanDtlList);
                        ppv.SMTEventsList.ForEach(i =>
                        {
                            i.resourceId = i.LineCode;
                            i.OldStartDt = i.StartDt;
                            i.id = i.OldId;
                        }); // set resourceId for each event
                        ppv.line_items = ppv.resources
                            .Select(i => new SelectListItem
                            {
                                Value = i.id,
                                Text = i.title
                            }).ToList();
                        //ppv.events = ppv.events.OrderBy(i => i.line).ThenBy(i => i.id).ToList();
                    }
                    Dictionary<string, object> para = await GetParas();
                    // model_change_time
                    if (para.TryGetValue("model_change_time", out var value))
                    {
                        ppv.model_stransfer_time = Convert.ToInt32(value);
                    }
                    // model_fisrt_run_percent
                    if (para.TryGetValue("model_fisrt_run_percent", out var value2))
                    {
                        ppv.new_model_rate = Convert.ToInt32(value2);
                    }

                }
                else
                {
                    ppv.prod_plans = new();
                    ppv.holidays = new();
                    ppv.resources = new();
                    ppv.events = new();
                    ppv.start_sch_dt = null;
                    ppv.line_items = new();
                }
                // Get all holidays
            }
            catch (Exception ex)
            {
                string t = ex.Message;
            }
            return ppv;
        }

        public async Task<string> SMTSaveProdPlan(ProdPlanViewModel ppv)
        {
            string msg = string.Empty;
            List<SMTProdPlanDtlModel> prodPlans = ppv.SMTProdPlanDtlList ?? new();
            List<SMTProdPlanHdrModel> prodPlansHdr = ppv.SMTProdPlanHdrList ?? new();
            if (prodPlans.Count == 0)
            {
                msg = "No production plan data to save.";
                return msg;
            }

            // This will avoid the case: delete --> timeout --> not add data yet
            using var transaction = await _con.Database.BeginTransactionAsync();

            try
            {
                // Step 1: Delete existing data
                var targetDate = Convert.ToDateTime(ppv.start_sch_dt).Date;
                // Step 1: Delete all records (if needed)
                List<int> listIds = prodPlans.Where(i=>i.StartScheduleDate == targetDate).Select(i => i.HeaderId).Distinct().ToList();
                await _con.Database.ExecuteSqlRawAsync(@"DELETE FROM UV_SMT_Prod_Plan_Dtl WHERE CAST(StartScheduleDate AS DATE) = {0};", targetDate);

                await _con.UV_SMT_Prod_Plan_Hdr.Where(i => listIds.Contains(i.Id)).ExecuteDeleteAsync();
                try
                {
                    var maxId = await _con.UV_SMT_Prod_Plan_Hdr.MaxAsync(p => (int?)p.Id) ?? 0;
                    maxId = maxId == 0 ? 0 : maxId + 1;
                    await _con.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT ('UV_SMT_Prod_Plan_Hdr', RESEED, {maxId})");
                    maxId = await _con.UV_SMT_Prod_Plan_Dtl.MaxAsync(p => (int?)p.Id) ?? 0;
                    maxId = maxId == 0 ? 0 : maxId + 1;
                    await _con.Database.ExecuteSqlRawAsync($"DBCC CHECKIDENT ('UV_SMT_Prod_Plan_Dtl', RESEED, {maxId})");
                }
                catch { }

                await _con.SaveChangesAsync();

                // Step 2: Prepare new data
                DateTime now = DateTime.Now;               

                prodPlansHdr.ForEach(i =>
                {
                    i.Id = 0;
                    i.CreatedBy = _hca.HttpContext?.User?.Identity?.Name ?? "system";
                    i.CreatedDt = now;
                });

                // Step 3: Insert new data
                await _con.UV_SMT_Prod_Plan_Hdr.AddRangeAsync(prodPlansHdr);
                await _con.SaveChangesAsync();

                // Map HeaderId to details
                //--------------->>>>>>>>>>> CHECKIT <<<<<<<<<<----------------
                var headerMap = prodPlansHdr.ToDictionary(i => (i.Lotno, i.PCBKey), i => i.Id);
                foreach (var d in prodPlans)
                {
                    if (headerMap.TryGetValue((d.Lotno, d.PCBKey), out var headerId))
                    {
                        d.HeaderId = headerId;
                    }
                    d.OldId = d.Id;
                    d.Id = 0;                    
                }
                await _con.UV_SMT_Prod_Plan_Dtl.AddRangeAsync(prodPlans);
                await _con.SaveChangesAsync();

                // Step 4: Commit transaction
                await transaction.CommitAsync();

                msg = "Production plan saved successfully.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                msg = "Error saving production plan: " + ex.Message;
            }

            try
            {

                if (ppv.holidays == null || !ppv.holidays.Any())
                {
                    return msg;
                }
                await addHolidays(ppv.holidays);

            }
            catch (Exception ex)
            {
                msg += $" Error saving holidays: {ex.Message}";
            }
            return msg;
        }

        public async Task<DataSet> SMTExportProdPlan(Dictionary<string, object> dic)
        {
            DataSet ds = new();
            DataSet dset = await _proc.Proc_GetDataset("PP_ProdPlan_Export", dic);
            if (dset != null && dset.Tables.Count > 1)
            {
                DataTable data = new();// dset.Tables[0];
                DataTable holiday = new();// dset.Tables[1];
                if (dset.Tables[0] != null && dset.Tables[0].Rows.Count > 0)
                {
                    if (dset.Tables[1].Rows.Count > 0)
                    {
                        holiday = dset.Tables[1].Copy();
                    }
                    holiday.TableName = "holiday";
                    ds.Tables.Add(holiday);

                    // remove the first row that is holiday
                    data = dset.Tables[0].Copy();
                    data.TableName = "COMB";
                    data.Rows.RemoveAt(0);
                    ds.Tables.Add(data);

                    var line_list = data.AsEnumerable()
                        .Select(row => row.Field<string>("line"))
                        .Distinct()
                        .ToList();
                    foreach (var line in line_list)
                    {
                        DataTable dt = data.Clone();
                        dt.TableName = line;
                        var rows = data.AsEnumerable().Where(r => r.Field<string>("line") == line).CopyToDataTable();
                        foreach (DataRow row in rows.Rows)
                        {
                            dt.ImportRow(row);
                        }
                        ds.Tables.Add(dt);
                    }

                }
            }
            return ds;
        }
        public async Task<ProdPlanViewModel> SMTReloadProdPlan(ProdPlanViewModel ppv)
        {
            try
            {
                // Step 1: Identify changes (line or start time)
                var changeLineDict = ppv.SMTEventsList
                    .Where(i => i.resourceId != i.LineCode || Convert.ToDateTime(i.start).AddHours(7).ToString("yyyy-MM-dd HH:mm:ss") != Convert.ToDateTime(i.OldStartDt).ToString("yyyy-MM-dd HH:mm:ss"))
                    .GroupBy(i => i.id)
                    .ToDictionary(
                        g => g.Key,
                        g => new SMTEventsDTO
                        {
                            id = g.Key,
                            LineCode = g.First().resourceId,
                            start = g.Min(j => j.start)
                        }
                    );

                // Step 2: Build main event list
                var ev = ppv.SMTEventsList
                    .GroupBy(i => new
                    {
                        i.Model,
                        i.Lotno,
                        i.PCBNo,
                        i.PCBKey,
                        i.PCBType,
                        i.PCBPerModel,

                        i.TargetPerHour85,
                        i.LotSize,
                        i.IssuedQty,
                        i.SortOrder,
                        i.backgroundColor,
                        i.borderColor
                    })
                    .Select(g =>
                    {
                        var first = g.First();
                        return new SMTEventsDTO
                        {
                            LineCode = first.resourceId,
                            MachineCode = first.MachineCode,
                            id = g.Max(k=>k.id),
                            SortOrder = g.Key.SortOrder,    
                            Model = g.Key.Model,
                            Lotno = g.Key.Lotno,
                            PCBNo = g.Key.PCBNo,
                            PCBKey = g.Key.PCBKey,
                            PCBType = g.Key.PCBType,
                            PCBPerModel = g.Key.PCBPerModel,
                            LotSize = g.Key.LotSize,
                            BalanceQty = g.Max(k => k.BalanceQty),
                            TargetPerHour85 = g.Key.TargetPerHour85,
                            start = g.Min(k => k.start),
                            StartDt = Convert.ToDateTime((g.Min(k => k.start))),
                            backgroundColor = g.Key.backgroundColor,
                            borderColor = g.Key.borderColor
                        };
                    })
                    .Select(e =>
                    {
                        if (changeLineDict.TryGetValue(e.id, out var change))
                        {
                            e.LineCode = change.LineCode;
                            e.start = change.start;
                        }
                        return e;
                    })
                    .ToList();


                List<SMTProdPlanDTO> prodPlans = _mapper.Map<List<SMTProdPlanDTO>>(ev);
                
                ppv.SMTProdPlanDTOList = prodPlans.OrderBy(i => i.LineCode).ThenBy(i => i.StartDt).ToList();
                ppv = await SMTCalProdPlan_AfterDragDrop(ppv);
                ppv = await SMTProdPlanViewModel(ppv);

            }
            catch (Exception ex)
            {
                return ppv;
            }

            return ppv;
        }

        public async Task<ProdPlanViewModel> SMTProdPlanViewModel(ProdPlanViewModel ppv)
        {
            //ppv = await CalProdPlan(ppv);

            //ppv.prod_plans = ppm;
            List<SMTProdPlanDTO> ppm = ppv.SMTProdPlanDTOList ?? new();

            if (ppm.Count>0)
            {
                ppv.resources = ppm.GroupBy(i => i.LineCode).Select(i => new ResourcesDTO
                {
                    id = i.Key,
                    title = i.Key
                }).ToList();


                ppv.SMTEventsList = _mapper.Map<List<SMTEventsDTO>>(ppm);
                //ppv.SMTEventsList.ForEach(i =>
                //{
                //    i.start =  i.StartDt.ToString("yyyy-MM-ddTHH:mm:ss");
                //    i.end =  i.EndDt.ToString("yyyy-MM-ddTHH:mm:ss");
                //    i.id = i.Id;
                //    i.resourceId = i.LineCode;
                //    i.OldStartDt = i.StartDt;
                //}); // set resourceId for each event
                ppv.line_items = ppv.resources
                    .Select(i => new SelectListItem
                    {
                        Value = i.id,
                        Text = i.title
                    }).ToList();

            }
            return ppv;
        }
        public async Task<ProdPlanViewModel> SMTCalProdPlan_AfterDragDrop(ProdPlanViewModel ppv)
        {
            var input = ppv.SMTProdPlanDTOList ?? new List<SMTProdPlanDTO>();
            if (!input.Any()) return ppv;

            // Setup minute
            ppv.SettingMinute = Convert.ToInt32(_con.UV_Common_Project_Setting
                .Where(x => x.Property == "SettingTime")
                .Select(x => x.Value)
                .First());

            // preload shift segments
            var shiftSegs = await _con.UV_SMT_Mst_Shift_Dtl.AsNoTracking()
                .Select(x => new SMTShiftSeg
                {
                    ShiftCode = x.ShiftCode,
                    StartMinute = x.StartMinute,
                    EndMinute = x.EndMinute
                })
                .OrderBy(x => x.StartMinute)
                .ToListAsync();

            var allShiftCodes = shiftSegs.Select(x => x.ShiftCode).Distinct().ToList();

            var lines = input.Select(x => x.LineCode).Distinct().ToList();

            var baseStart = (ppv.start_sch_dt ?? ppv.sch_dt ?? DateTime.Now.Date).Date;

            var result = new List<SMTProdPlanDTO>();

            int maxLookAheadDays = 180; // add this property if needed

            foreach (var line in lines)
            {
                // Expand line calendar
                var lcExpanded = await BuildLineCalendarExpandedAsync(line);
                if (!lcExpanded.Any())
                    continue;

                // Line state
                var st = new LineState
                {
                    Cursor = baseStart,
                    PrevPCBKey = null,
                    SetupMinute = ppv.SettingMinute ?? 0
                };

                // Cache windows by date for this line
                var windowsCache = new Dictionary<DateTime, List<ShiftWindow>>();

                // IMPORTANT: avoid double count if UI sends multiple rows per Lot+PCBKey
                // If UI already sends 1 row per Lot+Key, you can skip GroupBy.
                var reqs = input.Where(x => x.LineCode == line)
                    .GroupBy(x => new { x.Lotno, x.PCBKey })
                    .Select(g =>
                    {
                        var first = g.OrderBy(x => x.SortOrder).First();
                        first.BalanceQty = g.Max(x => x.BalanceQty) ?? 0; // since balance is total already; choose max
                                                                          // or Sum if UI splits balance (but chị nói "tổng theo Lot/Key" => max is safer)
                        return first;
                    })
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                foreach (var req in reqs)
                {
                    int remaining = req.BalanceQty ?? 0;
                    if (remaining <= 0) continue;

                    // normalize base cursor to allowed date if required
                    st.Cursor = GetNextAllowedDate(st.Cursor.Date, lcExpanded);

                    await ScheduleRequirementRecursiveAsync(
                        ppv,
                        req,
                        line,
                        st,
                        lcExpanded,
                        shiftSegs,
                        allShiftCodes,
                        windowsCache,
                        result,
                        remainingQty: remaining,
                        firstSegment: true,
                        lookAheadDays: 0,
                        maxLookAheadDays: maxLookAheadDays
                    );
                }
            }

            // Re-sort for UI timeline
            ppv.SMTProdPlanDTOList = result
                .OrderBy(x => x.StartScheduleDate)
                .ThenBy(x => x.LineCode)
                .ThenBy(x => x.StartDt)
                .ToList();

            return ppv;
        }

        


        private static IEnumerable<string> SplitCsvOrStar(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return Enumerable.Empty<string>();
            raw = raw.Trim();
            if (raw == "*") return new[] { "*" };

            return raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(x => x.Trim())
                      .Where(x => x.Length > 0);
        }

        public async Task<List<SMTLineCalendarExpanded>> BuildLineCalendarExpandedAsync(string line)
        {
            // join header+detail, filter line or "*", order by priority
            var src = await _con.UV_SMT_Mst_LineCalendar_Dtl
                .Include(x => x.SMTLineCalendarHdrModel)
                .Where(x =>
                    x.SMTLineCalendarHdrModel.IsActive &&
                    (("," + x.SMTLineCalendarHdrModel.LineCode + ",").Contains("," + line + ",")
                     || x.SMTLineCalendarHdrModel.LineCode == "*") &&
                    !string.IsNullOrEmpty(x.SMTLineCalendarHdrModel.ShiftCode)
                )
                .OrderBy(x => x.SMTLineCalendarHdrModel.Priority)
                .Select(x => new
                {
                    LineCodeRaw = x.SMTLineCalendarHdrModel.LineCode,
                    ShiftCodeRaw = x.SMTLineCalendarHdrModel.ShiftCode,
                    x.ConditionType,
                    x.WeekDay,
                    x.StartDate,
                    x.EndDate,
                    Priority = x.SMTLineCalendarHdrModel.Priority
                })
                .ToListAsync();

            var result = new List<SMTLineCalendarExpanded>();

            foreach (var s in src)
            {
                var lineTokens = SplitCsvOrStar(s.LineCodeRaw);
                var shiftTokens = SplitCsvOrStar(s.ShiftCodeRaw);

                foreach (var lc in lineTokens)
                    foreach (var sc in shiftTokens)
                    {
                        // Match SQL CASE: if raw == "*" keep "*", else token
                        result.Add(new SMTLineCalendarExpanded
                        {
                            LineCodeOne = lc,
                            ShiftCodeOne = sc,
                            ConditionType = s.ConditionType,
                            WeekDay = s.WeekDay,
                            StartDt = s.StartDate?.Date,
                            EndDt = s.EndDate?.Date,
                            Priority = s.Priority
                        });
                    }
            }

            return result;
        }


        // GET property date base on Line Calendar
        private static bool IsAllowedDate(DateTime date, IEnumerable<SMTLineCalendarExpanded> lc)
        {
            var d = date.Date;
            var ddd = date.ToString("ddd"); // "Mon","Tue"... tùy culture; nếu muốn chắc chắn, set CultureInfo("en-US")

            return lc.Any(i =>
                (i.ConditionType == "Weekday" && i.WeekDay == ddd)
                || (i.ConditionType == "Date" && i.StartDt.HasValue && i.StartDt.Value.Date == d)
                || (i.ConditionType == "Range" && i.StartDt.HasValue && i.EndDt.HasValue
                                           && d >= i.StartDt.Value.Date && d <= i.EndDt.Value.Date)
            );
        }

        private static DateTime GetNextAllowedDate(DateTime date, List<SMTLineCalendarExpanded> lc)
        {
            // Nếu không allowed -> nhảy ngày (đệ quy)
            return IsAllowedDate(date, lc) ? date.Date : GetNextAllowedDate(date.AddDays(1), lc);
        }

        // Pick property Shift
        private static List<string> PickAllowedShifts(
                                        string line, DateTime planDate,
                                        List<SMTLineCalendarExpanded> expanded,
                                        List<string> allShiftCodes)
        {
            bool Match(SMTLineCalendarExpanded r)
            {
                if (!(r.LineCodeOne == line || r.LineCodeOne == "*")) return false;
                var d = planDate.Date;
                var ddd = planDate.ToString("ddd");

                return (r.ConditionType == "Weekday" && r.WeekDay == ddd)
                    || (r.ConditionType == "Date" && r.StartDt.HasValue && r.StartDt.Value.Date == d)
                    || (r.ConditionType == "Range" && r.StartDt.HasValue && r.EndDt.HasValue
                                               && d >= r.StartDt.Value.Date && d <= r.EndDt.Value.Date);
            }

            // Pick one between HC and HCTruc
            var pickHC = expanded
                .Where(r => (r.ShiftCodeOne == "HC" || r.ShiftCodeOne == "HCTruc") && Match(r))
                .OrderBy(r => r.Priority)
                .ThenBy(r => r.LineCodeOne == line ? 0 : 1) // line-specific wins
                .Select(r => r.ShiftCodeOne)
                .FirstOrDefault();

            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(pickHC))
                allowed.Add(pickHC);

            // Other shifts
            foreach (var sh in allShiftCodes.Where(s => s != "HC" && s != "HCTruc"))
            {
                if (expanded.Any(r => Match(r) && (r.ShiftCodeOne.Equals(sh, StringComparison.OrdinalIgnoreCase) || r.ShiftCodeOne == "*")))
                    allowed.Add(sh);
            }

            return allowed.ToList();
        }

        // buil shift windows for a day
        private sealed class ShiftWindow
        {
            public string ShiftCode { get; set; } = "";
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public int WindowMinutes { get; set; }
        }

        private static List<ShiftWindow> BuildWindowsForDate(
            DateTime date,
            List<SMTShiftSeg> shiftSegs,
            List<string> allowedShiftCodes)
        {
            var allowed = new HashSet<string>(allowedShiftCodes, StringComparer.OrdinalIgnoreCase);

            return shiftSegs
                .Where(s => allowed.Contains(s.ShiftCode))
                .Select(s =>
                {
                    var start = date.Date.AddMinutes(s.StartMinute);
                    var end = date.Date.AddMinutes(s.EndMinute);
                    if (s.EndMinute < s.StartMinute) end = end.AddDays(1);

                    int mins = s.EndMinute >= s.StartMinute
                        ? s.EndMinute - s.StartMinute
                        : (s.EndMinute + 1440) - s.StartMinute;

                    return new ShiftWindow
                    {
                        ShiftCode = s.ShiftCode,
                        Start = start,
                        End = end,
                        WindowMinutes = mins
                    };
                })
                .OrderBy(w => w.Start)
                .ToList();
        }



        private DateTime SMTGetDate(DateTime date, List<SMTLineCalendarDTO> lc)
        {
            bool isAllowed = lc.Any(i =>
                (i.ConditionType == "Weekday" && date.ToString("ddd") == i.Weekday)
                || (i.ConditionType == "Date" && i.StartDt.HasValue && date.Date == i.StartDt.Value.Date)
                || (i.ConditionType == "Range" && i.StartDt.HasValue && i.EndDt.HasValue
                                          && date.Date >= i.StartDt.Value.Date && date.Date <= i.EndDt.Value.Date)
            );

            // Nếu KHÔNG allowed => nhảy ngày
            return isAllowed ? date : SMTGetDate(date.AddDays(1), lc);
        }

        // -- Helper --
        private static SMTProdPlanDTO SMTCreateProdPlan(
            SMTProdPlanDTO p, int Id, DateTime start, DateTime end,
            string line, DateTime startSchDate,
            int qty, int balQty, int setupMinute)
                {
            int minutes = (int)Math.Max(0, Math.Ceiling((end - start).TotalMinutes));

            return new SMTProdPlanDTO
            {
                //RowId = p.RowId,
                SortOrder = p.SortOrder,
                Id = Id,
                    
                Market = null, // nếu có
                Model = p.Model,
                PCBKey = p.PCBKey,
                Lotno = p.Lotno,
                PCBType = p.PCBType,
                PCBNo = p.PCBNo,

                LineCode = line,
                MachineCode = p.MachineCode,

               // Program_Name = p.Program_Name,
                RunOrder = p.RunOrder,

                StartScheduleDate = startSchDate,
                StartDt = start,
                EndDt = end,

                Qty = qty,
                BalQty = balQty,
                BalanceQty = p.BalanceQty, // original remaining
                TargetPerHour85 = p.TargetPerHour85,
                PCBPerModel = p.PCBPerModel,
                LotSize = p.LotSize,
                IssuedQty = p.IssuedQty,

                TimeTotal = minutes,
                SetupMinute = setupMinute,

                backgroundColor = p.backgroundColor,
                borderColor = p.borderColor,
                OldId = p.Id,
                Remark = p.Remark
            };
        }
        private sealed class LineState
        {
            public DateTime Cursor { get; set; }
            public string? PrevPCBKey { get; set; }
            public int SetupMinute { get; set; }
            public double OverMinute { get; set; } = 0;
        }
        int id = 0;

        private async Task ScheduleRequirementRecursiveAsync(
                                                            ProdPlanViewModel ppv,
                                                            SMTProdPlanDTO req,
                                                            string line,
                                                            LineState st,
                                                            List<SMTLineCalendarExpanded> lcExpanded,
                                                            List<SMTShiftSeg> shiftSegs,
                                                            List<string> allShiftCodes,
                                                            Dictionary<DateTime, List<ShiftWindow>> windowsCache,
                                                            List<SMTProdPlanDTO> result,
                                                            int remainingQty,
                                                            bool firstSegment,
                                                            int lookAheadDays,
                                                            int maxLookAheadDays)
        {
            if (remainingQty <= 0) return;

            if (lookAheadDays > maxLookAheadDays)
                throw new InvalidOperationException($"Exceeded max look-ahead days ({maxLookAheadDays}) while scheduling line {line}. Check LineCalendar/Shift.");

            // 1) Auto choose machine/output when first segment
            //if (firstSegment)
            //{
            //    //var best = await ResolveMachineForLineAsync(req.Model, req.PCBNo, line, req.Remark);
            //    req.MachineCode = best.MachineCode;
            //    req.TargetPerHour85 = best.OutputPerHour;
            //    req.RunOrder = best.RunOrder;
            //    //req.Program_Name = best.ProgramName;
            //}

            // 2) Setup if PCBKey changes (only once at first segment)
            int setup = 0;
            double addMinute = 0;
            if (firstSegment && st.PrevPCBKey != null &&
                !string.Equals(st.PrevPCBKey.Trim(), req.PCBKey.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                setup = st.SetupMinute;
                st.Cursor = st.Cursor.AddMinutes(setup);
                addMinute = setup - st.OverMinute >0 ? setup - st.OverMinute : 0;
            }
            else
            {
                addMinute = 0;
            }

            // 3) Ensure cursor on allowed date for this line
            var nextAllowed = GetNextAllowedDate(st.Cursor.Date, lcExpanded);
            if (st.Cursor < nextAllowed) st.Cursor = nextAllowed;

            // 4) Build windows for day if not cached
            var day = st.Cursor.Date;
            if (!windowsCache.TryGetValue(day, out var windows))
            {
                var allowed = PickAllowedShifts(line, day, lcExpanded, allShiftCodes);
                windows = BuildWindowsForDate(day, shiftSegs, allowed);
                windowsCache[day] = windows;
            }

            // 5) Find next usable window
            var win = windows.FirstOrDefault(w => w.End > st.Cursor);

            // If no window today => move to next day
            if (win == null)
            {
                st.Cursor = st.Cursor.Date.AddDays(1);
                await ScheduleRequirementRecursiveAsync(ppv, req, line, st, lcExpanded, shiftSegs, allShiftCodes,
                    windowsCache, result, remainingQty, firstSegment,
                    lookAheadDays + 1, maxLookAheadDays);
                return;
            }

            // move cursor into window
            if (st.Cursor < win.Start) st.Cursor = win.Start.AddMinutes(addMinute);

            var availMin = (win.End - st.Cursor).TotalMinutes;
            if (availMin <= 0)
            {
                st.Cursor = win.End;
                await ScheduleRequirementRecursiveAsync(ppv, req, line, st, lcExpanded, shiftSegs, allShiftCodes,
                    windowsCache, result, remainingQty, firstSegment,
                    lookAheadDays, maxLookAheadDays);
                return;
            }

            // 6) Compute minutes needed for remaining qty
            int outPerHour = Math.Max(1, req.TargetPerHour85 ?? 1);
            double needMin = remainingQty / (double)outPerHour * 60.0;

            double useMin = Math.Min(needMin, availMin);

            // optional cap per segment

            if (useMin <= 0) return;

            int segQty = (int)Math.Ceiling(useMin * outPerHour / 60.0);
            if (segQty > remainingQty) segQty = remainingQty;
            if (segQty <= 0) segQty = remainingQty;

            var start = st.Cursor;
            var end = st.Cursor.AddMinutes(Math.Ceiling(useMin));
            id++;
            result.Add(SMTCreateProdPlan(
                req,id, start, end, line, start.Date,
                qty: segQty,
                balQty: remainingQty - segQty,
                setupMinute: firstSegment ? setup : 0
            ));

            st.Cursor = end;
            st.OverMinute = (win.End - st.Cursor).TotalMinutes;

            int newRemain = remainingQty - segQty;

            if (newRemain > 0)
            {
                await ScheduleRequirementRecursiveAsync(ppv, req, line, st, lcExpanded, shiftSegs, allShiftCodes,
                    windowsCache, result, newRemain, false,
                    lookAheadDays, maxLookAheadDays);
            }
            else
            {
                st.PrevPCBKey = req.PCBKey;
            }
        }      


        #endregion

        #endregion

    }
}
/*
 * 
 SMTProdPlanDTO SMTCreateProdPlan(SMTProdPlanDTO p, DateTime start, DateTime end,  string line, DateTime startSchDate, int? qty = null, int? BalQty=null, int? SetupMinute = null)
        {
            return new SMTProdPlanDTO
            {
                Market = p.Market,
                Model = p.Model,
                PCBKey = p.PCBKey,
                Lotno = p.Lotno,
                PCBType = p.PCBType,
                PCBNo = p.PCBNo,
                LineCode = line ?? p.LineCode,
                MachineCode = p.MachineCode,
                KeyCode = p.KeyCode,
                PCBPerModel = p.PCBPerModel,
                LotSize = p.LotSize,
                IssuedQty = p.IssuedQty,
                BalanceQty = p.BalanceQty,
                TargetPerHour85 = p.TargetPerHour85,
                TargetPerShift = p.TargetPerShift,
                TimeF = p.TimeF,
                Remark = p.Remark,
                PlanCompletedDt = p.PlanCompletedDt,

                StartScheduleDate = startSchDate,
                StartDt = start,
                EndDt = end,

                BalQty = BalQty ?? 0,
                Qty = qty ?? p.Qty,
                TimeTotal = p.TimeTotal,
                SortOrder = p.SortOrder,
                backgroundColor = p.backgroundColor,
                borderColor = p.borderColor,
                OldId = p.OldId,

                RunOrder = p.RunOrder,
                SetupMinute = SetupMinute ?? 0
            };
        }
public async Task<ProdPlanViewModel> SMTCalProdPlan(ProdPlanViewModel ppv)
        {
           

            //List<ProdPlanModel> ppm = new();
            List<SMTProdPlanDTO> _ppm = ppv.SMTProdPlanDTOList ?? new();

            List<string> lines = new();
            if (_ppm != null && _ppm.Any())
                lines = _ppm.GroupBy(i => i.LineCode).Select(i => i.Key).ToList();

            DateTime sch_dt = ppv.sch_dt ?? DateTime.Now.Date;
            foreach (var line in lines)
            {
                ppv.line = line;
                ppv.sch_dt = sch_dt;
                ppv.is_other_model = false;
                ppv.start_sch_dt = ppv.start_sch_dt ?? DateTime.Now.Date;
                ppv.SettingMinute = Convert.ToInt32( _con.UV_Common_Project_Setting.Where(i => i.Property == "SettingTime").First().Value);
                foreach (var p in _ppm.Where(i => i.LineCode == line).ToList())
                {
                    ppv.SMTProdPlanDTO = p;                    
                    ppv.line = p.LineCode;
                    await _SMTCalProdPlan(ppv);
                }
            }
            ppv.prod_plans = ppm;
            return ppv;
        }

        public async Task<ProdPlanViewModel> _SMTCalProdPlan(ProdPlanViewModel ppv)  
        {
            //CHECK IF ANY PCBKey with RunOrder

            SMTProdPlanDTO p = ppv.SMTProdPlanDTO;
            string line = ppv.line;
            DateTime start_sch_date = Convert.ToDateTime(ppv.start_sch_dt);

            // GET Line Calendar
            List<SMTLineCalendarDTO> LineCalendarDTOList = new();
            var LineCalendar = _con.UV_SMT_Mst_LineCalendar_Dtl.Include(i => i.SMTLineCalendarHdrModel)
                                .Where(i =>
                                            ("," + i.SMTLineCalendarHdrModel.LineCode + ",")
                                                .Contains("," + line + ",")
                                            ||
                                            i.SMTLineCalendarHdrModel.LineCode == "*"
                                        )
                                .OrderBy(i => i.SMTLineCalendarHdrModel.Priority);
            
            if (LineCalendar.Any()) return ppv;
            int minPriority = LineCalendar.OrderBy(i => i.SMTLineCalendarHdrModel.Priority).First().SMTLineCalendarHdrModel.Priority;
            var lc = LineCalendar.Where(i => i.SMTLineCalendarHdrModel.Priority == minPriority);
            

            var ShiftList = lc.First().SMTLineCalendarHdrModel.ShiftCode
                                                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                                                .Select(x => x.Trim());
            foreach(var l in lc)
            foreach(var sl in ShiftList)
            {
                LineCalendarDTOList.Add(new SMTLineCalendarDTO()
                {
                    LineCodeOne = line
                    ,ShiftCodeOne = sl
                    ,Priority = l.SMTLineCalendarHdrModel.Priority
                    ,CondtitionType = l.ConditionType
                    ,Weekday = l.WeekDay
                    ,StartDt = l.StartDate?.Date
                    ,EndDt = l.EndDate?.Date
                });
            }

            // Choose property date belongs to this calendar here
            DateTime dt = SMTGetDate(ppv.sch_dt ?? DateTime.Now, LineCalendarDTOList).Date;


            List<string> shiftString = LineCalendarDTOList.Select(i => i.ShiftCodeOne).ToList();

            // GET Shift segment
            var SMTShiftList = _con.UV_SMT_Mst_Shift_Dtl.Where(i=> shiftString.Contains(i.ShiftCode)).OrderBy(i => i.StartMinute).ToList();

            
            foreach( var ssl in SMTShiftList)
            {
                
                DateTime start_run = dt.AddMinutes(ssl.StartMinute);
                DateTime end_run = dt.AddMinutes(ssl.EndMinute);

                double NeedMinutes = (double)(p.BalanceQty ?? 0) / ((p.TargetPerHour85 ?? 1) * 60);
                
                if(start_run.AddMinutes(NeedMinutes) > end_run)
                {

                }
                else
                {

                }

            }


            return ppv;
        } 
 */