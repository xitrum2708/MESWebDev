using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Models.Setting;
using MESWebDev.Models.Setting.DTO;
using MESWebDev.Services.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Drawing;
using System.Threading;

namespace MESWebDev.Services
{
    public class SettingService : ISettingService
    {
        private readonly AppDbContext _con;
        private readonly IMemoryCache _cache;
        private readonly Procedure _proc;
        private readonly IHttpContextAccessor _hca;
        public SettingService(AppDbContext context, IMemoryCache cache, IHttpContextAccessor hca)
        {
            _con = context;
            _cache = cache;
            _proc = new Procedure(_con);
            _hca = hca;
        }

        public async Task<FormatRazorDTO> GetFormatRazor()
        {
            FormatRazorDTO fr = new();
            #pragma warning disable CS8603 // Possible null reference return.
            return await _cache.GetOrCreateAsync(SD.CatchSettingKey, async entry =>
            {
                entry.Size = 1;
                entry.SlidingExpiration = TimeSpan.FromMinutes(SD.TimeOut);
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(SD.TimeOut * 10);
                entry.Priority = CacheItemPriority.High;

                var data = await _con.UV_Common_Project_Setting.Where(i => i.IsActive).AsNoTracking().ToListAsync();
                if (data.Any()) {
                    var NumberFormat = data.Where(i => i.Property == SD.FormatNumber);
                    if (NumberFormat.Any()) fr.NumberFormat = NumberFormat.First().Value;

                    var DateFormat = data.Where(i => i.Property == SD.FormatDate);
                    if (DateFormat.Any()) fr.DateFormat = DateFormat.First().Value;

                    var DatetimeFormat = data.Where(i => i.Property == SD.FormatDatetime);
                    if (DatetimeFormat.Any()) fr.DatetimeFormat = DatetimeFormat.First().Value;

                    var NumberCss = data.Where(i => i.Property == SD.CssNumber);
                    if (NumberCss.Any()) fr.NumberCss = NumberCss.First().Value;

                    var DateCss = data.Where(i => i.Property == SD.CssDate);
                    if (DateCss.Any()) fr.DateCss = DateCss.First().Value;

                    var StringFormat = data.Where(i => i.Property == SD.CssString);
                    if (StringFormat.Any()) fr.TextCss = StringFormat.First().Value;
                }
                return fr;
            });
            #pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task<List<ProjectSettingModel>> GetSettingList()
        {

            #pragma warning disable CS8603 // Possible null reference return.
            return await _cache.GetOrCreateAsync(SD.CatchSettingKey, async entry =>
            {
                // Cache policy
                entry.SlidingExpiration = TimeSpan.FromMinutes(SD.TimeOut);           
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(SD.TimeOut * 2); 
                entry.Priority = CacheItemPriority.High;

                // query EF Core: avoid cache entity/raw
                var settings = await _con.UV_Common_Project_Setting
                    .Where(s => s.IsActive)
                    .AsNoTracking() // read-only
                    .Select(s => new ProjectSettingModel
                    {
                        Id = s.Id,
                        Property = s.Property,
                        Value = s.Value
                    })
                    .ToListAsync();

                return settings;
            });
            #pragma warning restore CS8603 // Possible null reference return.
        }

        public void ReloadSetting()
        {
            _cache.Remove(SD.CatchSettingKey);
        }


        #region COMMON Setting
        public async Task<DataTable> ProjectSettingList(Dictionary<string, object> dic)
        {
            DataTable dt = new();
            dt = await _proc.Proc_GetDatatable("spweb_UV_Common_ProjectSetting", dic);
            return dt;
        }

        public async Task<string> ProjectSettingAdd(ProjectSettingModel psModel)
        {
            //Add Project Setting
            string msg = string.Empty;
            try
            {
                var check = _con.UV_Common_Project_Setting.Where(i => i.Property == psModel.Property && i.IsActive == true);
                if (check.Any())
                {
                    return "This property existed !";
                }
                psModel.CreatedBy = _hca.HttpContext.User.Identity.Name ?? "system";
                psModel.CreatedDt = DateTime.Now;
                await _con.UV_Common_Project_Setting.AddAsync(psModel);
                await _con.SaveChangesAsync();
                //reload catch
                ReloadSetting();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }
        public async Task<string> ProjectSettingEdit(ProjectSettingModel psModel)
        {
            //add Project Setting
            string msg = string.Empty;
            try
            {
                ProjectSettingModel? data = await _con.UV_Common_Project_Setting.FindAsync(psModel.Id);
                if (data != null)
                {
                    data.Value = psModel.Value;
                    data.Category = psModel.Category;
                    data.IsActive = psModel.IsActive;
                    data.Remark = psModel.Remark;
                    data.UpdatedBy = _hca.HttpContext.User.Identity.Name ?? "system";
                    data.UpdatedDt = DateTime.Now;
                    await _con.SaveChangesAsync();

                    //reload catch
                    ReloadSetting();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }

        public async Task<string> ProjectSettingDelete(int id)
        {
            string msg = string.Empty;
            try
            {
                ProjectSettingModel? data = await _con.UV_Common_Project_Setting.FindAsync(id);
                if (data != null)
                {
                    data.IsActive = false;
                    data.UpdatedBy = _hca.HttpContext.User.Identity.Name ?? "system";
                    data.UpdatedDt = DateTime.Now;
                    await _con.SaveChangesAsync();
                    //reload catch
                    ReloadSetting();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }

        public async Task<ProjectSettingModel> ProjectSettingDetail(int id)
        {
            return await _con.UV_Common_Project_Setting.FindAsync(id) ?? new ProjectSettingModel();
        }

        public async Task<string> ProjectSettingValue(string property)
        {
            var data = _con.UV_Common_Project_Setting.Where(i => i.Property == property);
            if (data.Any())
            {
                return data.First().Value;
            }
            return string.Empty;
        }
        #endregion
    }
}
