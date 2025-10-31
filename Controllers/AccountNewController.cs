using MESWebDev.Data;
using MESWebDev.Models.VM;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace MESWebDev.Controllers
{
    [AllowAnonymous]
    public class AccountNewController : Controller
    {
        private readonly AppDbContext _context;

        public AccountNewController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string languageCode = "en") // Mặc định là 'vi'
        {
            // Lấy danh sách ngôn ngữ từ database
            var languages = _context.Master_Language.Where(l => l.IsActive).ToList();

            // Lưu ngôn ngữ vào Session hoặc Cookie
            HttpContext.Session.SetString("LanguageCode", languageCode);

            // Lấy ngôn ngữ hiện tại để hiển thị trên dropdown
            ViewBag.Languages = languages;
            ViewBag.SelectedLanguage = languageCode;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string languageCode)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Username or password cannot be empty";
                ViewBag.Languages = _context.Master_Language.Where(l => l.IsActive).ToList();
                ViewBag.SelectedLanguage = languageCode;
                return View();
            }

            // Truy vấn user với kiểm tra NULL
            var user = _context.Auth_Master_User
                .Where(u => u.Username == username && u.Password == password)
                .Select(u => new
                {
                    u.Username,
                    u.Password,
                    u.Email,
                    u.Fullname,
                    u.LangId,
                    u.IsActive,
                    u.CreatedDt
                })
                .FirstOrDefault();

            if (user == null)
            {
                ViewBag.Error = "Invalid credentials";
                ViewBag.Languages = _context.Languages.Where(l => l.IsActive).ToList();
                ViewBag.SelectedLanguage = languageCode;
                return View();
            }
            // Add cookie-based authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
            });
            // Lưu thông tin vào session
            //HttpContext.Session.SetInt32("UserId", user.Username);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("LanguageCode", languageCode);

            // 🔥 Save Menu Cache vào Session
            await SaveMenuToSession(user.Username);
            // Cập nhật ngôn ngữ mặc định của user (nếu cần)
            var userEntity = _context.Users.Find(user.Username);
            if (userEntity != null)
            {
                userEntity.LanguageId = _context.Languages
                    .Where(l => l.Code == languageCode)
                    .Select(l => (int?)l.LanguageId)
                    .FirstOrDefault();
                _context.SaveChanges();
            }

            return RedirectToAction("IQCDashboard", "Admin");
        }

        // --- SAVE MENU TO SESSION ---
        private async Task SaveMenuToSession(string username)
        {
            var menuTree = await GetUserMenuTree(username);

            var jsonMenu = JsonSerializer.Serialize(menuTree);

            HttpContext.Session.SetString($"UserMenu_{username}", jsonMenu);
        }

        // --- GET USER MENU TREE ---
        private async Task<List<MenuViewModel>> GetUserMenuTree(string Username)
        {
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "en";
            var languageId = await _context.Master_Language
                .Where(l => l.Culture == languageCode)
                .Select(l => l.Id)
                .FirstOrDefaultAsync();

            var menus = await _context.Auth_Master_User
                .Where(ur => ur.Username == Username)
                .Join(_context.Auth_Master_Role,
                    ur => ur.RoleId,
                    rp => rp.RoleId,
                    (ur, rp) => rp)
                .Join(_context.Auth_Mapping_Role_Func_Pms,
                    rp => rp.RoleId,
                    p => p.RoleId,
                    (rp, p) => p)
                .Join(_context.Auth_Master_Function,
                    p => p.FuncId,
                    m => m.Id,
                    (p, m) => m)
                .Where(m => m.IsActive)
                .OrderBy(m => m.Order)
                .Select(m => new MenuViewModel
                {
                    MenuId = m.Id,
                    Url = Url.Action(m.Action,m.Controller),
                    SortOrder = m.Order,
                    ParentId = m.ParentId,
                    Icon = m.IconString,
                    IsActive = m.IsActive,
                    Title = _context.Master_Language_Dic
                        .Where(mt => mt.Key == m.Id.ToString() && mt.LangId == languageId)
                        .Select(mt => mt.Value)
                        .FirstOrDefault() ?? "No Translation"
                })
                .ToListAsync();

            var menuTree = BuildMenuTree(menus);

            return menuTree;
        }

        private List<MenuViewModel> BuildMenuTree(List<MenuViewModel> menus, int? parentId = null)
        {
            return menus
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.SortOrder)
                .Select(m => new MenuViewModel
                {
                    MenuId = m.MenuId,
                    Url = m.Url,
                    SortOrder = m.SortOrder,
                    ParentId = m.ParentId,
                    Icon = m.Icon,
                    PermissionKey = m.PermissionKey,
                    IsActive = m.IsActive,
                    Title = m.Title,
                    Children = BuildMenuTree(menus, m.MenuId)
                })
                .ToList();
        }

        public IActionResult ChangeLanguage(string languageCode)
        {
            HttpContext.Session.SetString("LanguageCode", languageCode);
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}