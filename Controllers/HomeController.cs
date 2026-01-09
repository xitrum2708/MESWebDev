using MESWebDev.Data;
using MESWebDev.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MESWebDev.Controllers
{
    [AuthorizeLogin]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
               // return RedirectToAction("Login", "Account");
            }
            var Username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(Username))
            {
                return RedirectToAction("Login", "Account");
            }
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            var languageId = _context.Languages
                .Where(l => l.Code == languageCode)
                .Select(l => l.LanguageId)
                .FirstOrDefault();

            // Lấy danh sách menu mà user có quyền truy cập
            var menus = _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Join(_context.RolePermissions,
                    ur => ur.RoleId,
                    rp => rp.RoleId,
                    (ur, rp) => rp)
                .Join(_context.Permissions,
                    rp => rp.PermissionId,
                    p => p.PermissionId,
                    (rp, p) => p)
                .Join(_context.Menus,
                    p => p.PermissionKey,
                    m => m.PermissionKey,
                    (p, m) => m)
                .Where(m => m.IsActive)
                .OrderBy(m => m.SortOrder)
                .Select(m => new
                {
                    m.MenuId,
                    m.Url,
                    m.SortOrder,
                    Title = _context.MenuTranslations
                        .Where(mt => mt.MenuId == m.MenuId && mt.LanguageId == languageId)
                        .Select(mt => mt.Title)
                        .FirstOrDefault() ?? "No Translation"
                })
                .ToList();

            return View(menus);
        }
    }
}