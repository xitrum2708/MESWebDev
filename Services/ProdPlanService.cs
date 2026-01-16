using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using ExcelDataReader;
using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Models.COMMON;
using MESWebDev.Models.PE;
using MESWebDev.Models.ProdPlan;
using MESWebDev.Models.ProdPlan.PC;
using MESWebDev.Models.ProdPlan.SMT;
using MESWebDev.Models.Setting;
using MESWebDev.Services.IService;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;

namespace MESWebDev.Services
{
    public class ProdPlanService : IProdPlanService
    {
        private readonly AppDbContext _con;
        private readonly IMapper _mapper;
        private readonly Procedure _proc;
        private readonly IHttpContextAccessor _hca;
        private readonly ITranslationService _trans;

        public ProdPlanService(AppDbContext con, IMapper mapper, IHttpContextAccessor hca, ITranslationService translation)
        {
            _con = con;
            _mapper = mapper;
            _proc = new Procedure(_con);
            _hca = hca;
            _trans = translation;
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


        // Hàm đệ quy
        public async Task<ProdPlanViewModel> _CalProdPlan2(ProdPlanViewModel ppv)  //!!!!!!!!!! thay đổi model + 30 phút
        {
            ProdPlanModel pp = new();
            ProdPlanModel p = ppv.prodPlanModel;
            DateTime start_sch_date = Convert.ToDateTime(ppv.start_sch_dt);

            int new_model_rate = ppv.new_model_rate ?? 65; // default = 65% for new model
            DateTime dt = GetDate(ppv.sch_dt ?? DateTime.Now, ppv.holidays).Date;
            DateTime start_break = dt.AddHours(ppv.break_working_time ?? 12);
            DateTime start_run = dt.AddHours(ppv.start_hour ?? 8);
            int start_working_time = ppv.start_working_time ?? 8;

            // Check start_run
            bool is_other_model = ppm.Count() > 0 && ppm.Last().model == (ppv.prodPlanModel?.model ?? string.Empty) ? false : ppv.is_other_model ?? false;
            int model_stransfer_time = ppv.model_stransfer_time ?? 30;


            // working hours in a day
            int working_hour = ppv.working_hour ?? 8;

            // end of working hour
            DateTime endOfWorking = dt.AddHours(working_hour + 1 + start_working_time);

            // IF FPP and Run first time:
            if (p?.is_fpp == true && ppv.is_first_run_fpp_model == true && p.lot_size == p.bal_qty)
            {
                if (ppv.start_hour == 8 && is_other_model)
                {

                    pp = new()
                    {
                        line = ppv.line
                        ,
                        id = p.id
                        ,
                        working_hour = working_hour
                        ,
                        qty = Math.Min(p.capa_qty, p.qty)// p.bal_qty <= p.capa_qty ? p.bal_qty : p.capa_qty
                        ,
                        start = start_run
                        ,
                        end = endOfWorking
                        ,
                        model = p.model
                        ,
                        lot_no = p.lot_no
                        ,
                        lot_size = p.lot_size
                        ,
                        capa_qty = p.capa_qty
                        ,
                        bal_qty = p.bal_qty
                        ,
                        backgroundColor = p.backgroundColor
                        ,
                        borderColor = p.borderColor
                        ,
                        is_fpp = p.is_fpp
                        ,
                        is_new = p.is_new
                        ,
                        start_sch_dt = start_sch_date
                    };
                    ppm.Add(pp);

                    //GET NEW ppv here
                    ppv.sch_dt = start_run.Date.AddDays(1);
                    ppv.start_hour = 8;
                    ppv.prodPlanModel.bal_qty = p.bal_qty - pp.qty;
                    ppv.is_other_model = p.bal_qty <= p.capa_qty ? true : false;
                    ppv.is_first_run_fpp_model = false; // reset first run fpp model

                    // CALL RECURSIVE FUNCTION -- gọi hàm đệ quy
                    if (p.bal_qty > p.capa_qty)
                        await _CalProdPlan(ppv);
                }
                else
                {
                    ppv.sch_dt = ppm.Count() == 0 ? start_run : start_run.AddDays(1); // is_other_model == false in case other model and 
                    ppv.start_hour = 8;
                    ppv.is_other_model = true; // void the case next day but alreay set this is FALSE 
                    await _CalProdPlan(ppv);
                }
            }
            else
            {

                // If transfer to new model, add more time
                if (is_other_model)
                    start_run = start_run.AddMinutes(model_stransfer_time);

                // check if start run in Break time or not ??
                DateTime end_break = start_break.AddHours(1); // 1 hour break time
                if (start_run >= start_break && start_run < end_break)
                {
                    start_run = start_run.AddHours(1);
                }

                if (start_run >= endOfWorking)
                {
                    ppv.sch_dt = start_run.AddDays(1);
                    ppv.start_hour = 8 + (start_run - endOfWorking).TotalHours;
                    ppv.is_other_model = false; // already + 0.5 hour so change to false
                    await _CalProdPlan(ppv);
                }
                else
                {
                    try
                    {
                        // Check is new model and run the first time
                        double finish = ((double)p.bal_qty * 8) / (double)p.capa_qty;
                        double minus = start_run >= start_break ? 0 : 1;
                        int qty = (int)Math.Round(((endOfWorking - start_run).TotalHours - minus) / 8 * p.capa_qty);
                        if (p.is_new && ppv.is_first_run_new_model == true)
                        {
                            finish = (finish * 100) / (double)new_model_rate; // 65% is the rate of new model, can be changed
                            ppv.is_first_run_new_model = false;
                            qty = (int)Math.Round(((((endOfWorking - start_run).TotalHours - minus) / 8 * p.capa_qty) * new_model_rate) / 100);
                        }

                        DateTime finish_run = start_run.AddHours(finish);

                        // + 1 hour if break time in [start_run] && [finish_run]
                        if (start_run < start_break && finish_run > start_break)
                        {
                            finish_run = finish_run.AddHours(1);
                        }

                        if (finish_run > endOfWorking) // run a model more than 1 day
                        {
                            pp = new()
                            {
                                line = ppv.line
                                ,
                                id = p.id
                                ,
                                working_hour = working_hour
                                ,
                                qty = qty
                                ,
                                start = start_run
                                ,
                                end = endOfWorking
                                ,
                                model = p.model
                                ,
                                lot_no = p.lot_no
                                ,
                                lot_size = p.lot_size
                                ,
                                capa_qty = p.capa_qty
                                ,
                                bal_qty = p.bal_qty
                                ,
                                backgroundColor = p.backgroundColor
                                ,
                                borderColor = p.borderColor
                                ,
                                is_fpp = p.is_fpp
                                ,
                                is_new = p.is_new
                                ,
                                start_sch_dt = start_sch_date
                            };
                            ppm.Add(pp);

                            //GET NEW ppv here
                            ppv.sch_dt = start_run.Date.AddDays(1);
                            ppv.start_hour = 8;
                            ppv.prodPlanModel.bal_qty = p.bal_qty - pp.qty;
                            ppv.is_other_model = false;
                            // CALL RECURSIVE FUNCTION -- gọi hàm đệ quy
                            await _CalProdPlan(ppv);
                        }
                        else
                        {
                            // Check is new model
                            pp = new()
                            {
                                line = ppv.line
                                ,
                                id = p.id
                                ,
                                working_hour = working_hour
                                ,
                                qty = p.bal_qty
                                ,
                                start = start_run
                                ,
                                end = finish_run
                                ,
                                model = p.model
                                ,
                                lot_no = p.lot_no
                                ,
                                lot_size = p.lot_size
                                ,
                                capa_qty = p.capa_qty
                                ,
                                bal_qty = p.bal_qty
                                ,
                                backgroundColor = p.backgroundColor
                                ,
                                borderColor = p.borderColor
                                ,
                                is_fpp = p.is_fpp
                                ,
                                is_new = p.is_new
                                ,
                                start_sch_dt = start_sch_date
                            };

                            ppm.Add(pp);
                            //GET NEW ppv here
                            ppv.sch_dt = start_run.Date;
                            ppv.start_hour = (finish_run - start_run.Date).TotalHours;
                            ppv.is_other_model = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        string t = ex.Message;
                    }


                }
            }

            return ppv;
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
            try {
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
            catch(Exception ex)
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
            catch(Exception ex)
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
                            pcbNo = pcbNo.Length >= 6
                                            ? pcbNo.Substring(1, 2) + pcbNo.Substring(4, 6)
                                            : pcbNo;

                            string model = CommonFormat.GetValueOrDefault(reader, colMap, "Model", string.Empty);
                            model = model.Length >= 6
                                            ? model.Substring(0, 6)
                                            : model;
                            string lotno = CommonFormat.GetValueOrDefault(reader, colMap, "LotNo", string.Empty);
                            if(!lotno.Contains("FCST") && (model.StartsWith("BY2") || model.StartsWith("BY3") || !model.StartsWith("BY")))
                            {
                                SMTLotPcbModel slp = new()
                                {
                                    Lotno = lotno,
                                    PCBNo = pcbNo,
                                    Model = model,
                                    UploadedFile = uploadedFile,
                                    Remark = "Data from IFS System",
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
            check.Lotno = slp.Lotno;
            check.Remark = $"Updated by {_hca.HttpContext?.User?.Identity?.Name?? string.Empty} at {DateTime.Now:yyMMddHHmmss}";
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

            await _con.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> ShiftDelete(string shiftCode)
        {
            var data = await _con.UV_SMT_Mst_Shift.FindAsync(shiftCode);
            if (data == null) return $"{shiftCode} {_trans.Trans(SD.ErrorMsg.NotExisted)}";
            _con.UV_SMT_Mst_Shift.Remove(data);
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

            await _con.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<string> LineCalendarDelete(int Id)
        {
            var data = await _con.UV_SMT_Mst_LineCalendar.FindAsync(Id);
            if (data == null) return $"{Id.ToString()} {_trans.Trans(SD.ErrorMsg.NotExisted)}";
            _con.UV_SMT_Mst_LineCalendar.Remove(data);
            await _con.SaveChangesAsync();
            return string.Empty;
        }

        #endregion

        #endregion
               
    }
}
