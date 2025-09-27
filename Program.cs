using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Middleware;
using MESWebDev.Repositories;
using MESWebDev.Services;
using MESWebDev.Services.IService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.SubFolder)
    .AddDataAnnotationsLocalization();

builder.Services.AddSession();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("vi"),
        new CultureInfo("en"),         
    };
    options.DefaultRequestCulture = new RequestCulture("vi");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
    {
        var langCode = context.Request.Query["lang"].ToString();
        //var langCode = context.Request.HttpContext.Session.GetString("LanguageCode") ?? "vi";
        if (string.IsNullOrEmpty(langCode)) langCode = "en";
        return Task.FromResult(new ProviderCultureResult(langCode));
    }));
});
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Đăng ký IMemoryCache
builder.Services.AddMemoryCache(options =>
{
    // Tùy chỉnh cấu hình IMemoryCache (tùy chọn)
    options.SizeLimit = 1024; // Giới hạn kích thước cache (tính bằng số lượng mục, không phải byte)
    options.CompactionPercentage = 0.2; // Khi cache đầy, 20% dữ liệu ít sử dụng nhất sẽ bị xóa
});
// Đăng ký TranslationService
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddScoped<IUVAssyProductionRepository, UVAssyProductionRepository>();
builder.Services.AddScoped<IIQCReportsRepository, IQCReportsRepository>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
builder.Services.AddScoped<DownloadExcelExportService>();
builder.Services.AddScoped<SqlHelperService>();
builder.Services.AddScoped<IRepairResultRepository, RepairResultRepository>();
builder.Services.AddScoped<IRepairResultService, RepairResultService>();
builder.Services.AddScoped<IUV_LOTCONTROL_MASTER_Service, UV_LOTCONTROL_MASTER_Service>();
builder.Services.AddScoped<IUVASSY_PPRODUCT_HISTORY, UVASSY_PPRODUCT_HISTORY>();


builder.Services.AddScoped<IProdPlanService, ProdPlanService>();
builder.Services.AddScoped<ISMTService, SMTService>();
builder.Services.AddScoped<IDashboard, Dashboard>();
builder.Services.AddScoped<IPEService, PEService>();

// Logging is already configured in ASP.NET Core by default, but you can customize it
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    // Add other logging providers if needed (e.g., Serilog)
});

// Add authentication services (example using cookie authentication)
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login"; // Redirect unauthenticated users to /Account/Login
        options.AccessDeniedPath = "/Account/AccessDenied"; // Optional: Redirect for access denied
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

//------ EPPlus : add it to use freely --------------
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
//------------- Add Mapper here (NOTE: error maybe because of version of mapper) ------------\\
builder.Services.AddAutoMapper(typeof(MappingConfig));

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/Account/Login"; // Redirect unauthenticated users to /Account/Login
//        options.AccessDeniedPath = "/Account/AccessDenied"; // Optional: Redirect for access denied
//        options.SlidingExpiration = true; // Optional: Enable sliding expiration
//        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Set the expiration time for the cookie
//    });

var app = builder.Build();
app.UseMiddleware<RequestLoggingMiddleware>();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseRequestLocalization();

app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=Index}/{id?}");
    //name: "default",
    //pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
