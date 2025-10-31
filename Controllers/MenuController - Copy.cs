using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Models;
using MESWebDev.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Controllers
{
    public class MenuControllerOld : BaseController
    {
        //private readonly AppDbContext _context;

        public MenuControllerOld(AppDbContext context)
            : base(context)
        {
            // _context = context;
        }

        private async Task<List<MenuViewModel>> GetUserMenuTree()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return new List<MenuViewModel>();

            var menuSessionKey = $"UserMenu_{userId}";
            var cachedMenusJson = HttpContext.Session.GetString(menuSessionKey);

            if (!string.IsNullOrEmpty(cachedMenusJson))
            {
                // Deserialize lại từ JSON
                return System.Text.Json.JsonSerializer.Deserialize<List<MenuViewModel>>(cachedMenusJson);
            }

            // Nếu chưa có cache, lấy từ database
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            var languageId = _context.Languages
                .Where(l => l.Code == languageCode)
                .Select(l => l.LanguageId)
                .FirstOrDefault();

            var menus = await _context.UserRoles
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
                .Select(m => new MenuViewModel
                {
                    MenuId = m.MenuId,
                    Url = m.Url,
                    SortOrder = m.SortOrder,
                    ParentId = m.ParentId,
                    Icon = m.Icon,
                    PermissionKey = m.PermissionKey,
                    IsActive = m.IsActive,
                    Title = _context.MenuTranslations
                        .Where(mt => mt.MenuId == m.MenuId && mt.LanguageId == languageId)
                        .Select(mt => mt.Title)
                        .FirstOrDefault() ?? "No Translation"
                })
                .ToListAsync();

            // Build tree
            var menuTree = BuildMenuTree(menus);

            // Cache vào Session
            var jsonMenu = System.Text.Json.JsonSerializer.Serialize(menuTree);
            HttpContext.Session.SetString(menuSessionKey, jsonMenu);

            return menuTree;
        }

        // GET: Menu/Index
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchTerm = null)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            var languageId = _context.Languages
                .Where(l => l.Code == languageCode)
                .Select(l => l.LanguageId)
                .FirstOrDefault();

            // Lấy tất cả menus chưa phân trang
            var menus = await _context.Menus
                .Where(m => m.IsActive)
                .OrderBy(m => m.SortOrder)
                .Select(m => new MenuViewModel
                {
                    MenuId = m.MenuId,
                    Url = m.Url,
                    SortOrder = m.SortOrder,
                    PermissionKey = m.PermissionKey,
                    IsActive = m.IsActive,
                    ParentId = m.ParentId,
                    Icon = m.Icon,
                    Title = _context.MenuTranslations
                        .Where(mt => mt.MenuId == m.MenuId && mt.LanguageId == languageId)
                        .Select(mt => mt.Title)
                        .FirstOrDefault() ?? "No Translation"
                })
                .ToListAsync();

            // Áp dụng tìm kiếm nếu có
            if (!string.IsNullOrEmpty(searchTerm))
            {
                menus = menus
                    .Where(m => m.Title.Contains(searchTerm) ||
                                m.PermissionKey.Contains(searchTerm) ||
                                m.Url.Contains(searchTerm))
                    .ToList();
            }

            // Build cây hoàn chỉnh
            var menuTree = BuildMenuTree(menus);

            // Không phân trang ở đây (hoặc bạn tự paging bằng js nếu muốn sau này)
            var pagedMenus = new PagedResult<MenuViewModel>
            {
                Items = menuTree, // Full cây
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = menuTree.Count
            };

            return View(pagedMenus);
        }

        private List<MenuViewModel> BuildMenuTree(List<MenuViewModel> menus, int? parentId = null, int level = 0)
        {
            var rootMenus = menus
                   .Where(m => m.ParentId == parentId)
                   .OrderBy(m => m.SortOrder)
                   .ToList();

            foreach (var menu in rootMenus)
            {
                menu.Level = level;
                menu.Children = BuildMenuTree(menus, menu.MenuId, level + 1); // đệ quy cấp con
            }
            return rootMenus;
        }

        // GET: Menu/Create
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            var languageId = _context.Languages
                .Where(l => l.Code == languageCode)
                .Select(l => l.LanguageId)
                .FirstOrDefault();

            // Lấy danh sách menu cha
            var menus = _context.Menus
                .Where(m => m.IsActive)
                .OrderBy(m => m.SortOrder)
                .Select(m => new MenuViewModel
                {
                    MenuId = m.MenuId,
                    ParentId = m.ParentId,
                    Title = _context.MenuTranslations
                        .Where(mt => mt.MenuId == m.MenuId && mt.LanguageId == languageId)
                        .Select(mt => mt.Title)
                        .FirstOrDefault() ?? "No Translation"
                })
                .ToList();

            // Xây dựng cây menu cha
            var availableParents = BuildMenuTree(menus);
            var permissions = _context.Permissions
                    .OrderBy(p => p.PermissionKey)
                    .Select(p => new PermissionViewModel
                    {
                        PermissionKey = p.PermissionKey,
                        Description = p.Description
                    })
                    .ToList();
            var model = new MenuViewModel
            {
                AvailableParents = availableParents,
                AvailablePermissions = permissions,
                IsActive = true
            };

            return View(model);
        }

        // POST: Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuViewModel model)
        {
            if (ModelState.IsValid)
            {
                var menu = new Menu
                {
                    Url = model.Url,
                    SortOrder = model.SortOrder,
                    PermissionKey = model.PermissionKey,
                    IsActive = model.IsActive,
                    ParentId = model.ParentId,
                    Icon = model.Icon,
                    CreatedAt = DateTime.Now,
                };

                _context.Menus.Add(menu);
                await _context.SaveChangesAsync();

                // Thêm bản dịch mặc định
                var languageId1 = _context.Languages
                    .Where(l => l.Code == "vi")
                    .Select(l => l.LanguageId)
                    .FirstOrDefault();

                var translation = new MenuTranslation
                {
                    MenuId = menu.MenuId,
                    LanguageId = languageId1,
                    Title = model.Title
                };

                _context.MenuTranslations.Add(translation);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, lấy lại danh sách menu cha
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            var languageId = _context.Languages
                .Where(l => l.Code == languageCode)
                .Select(l => l.LanguageId)
                .FirstOrDefault();

            var menus = _context.Menus
                .Where(m => m.IsActive)
                .OrderBy(m => m.SortOrder)
                .Select(m => new MenuViewModel
                {
                    MenuId = m.MenuId,
                    ParentId = m.ParentId,
                    Title = _context.MenuTranslations
                        .Where(mt => mt.MenuId == m.MenuId && mt.LanguageId == languageId)
                        .Select(mt => mt.Title)
                        .FirstOrDefault() ?? "No Translation"
                })
                .ToList();

            model.AvailableParents = BuildMenuTree(menus);
            return View(model);
        }

        // GET: Menu/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
                return NotFound();

            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            var languageId = _context.Languages
                .Where(l => l.Code == languageCode)
                .Select(l => l.LanguageId)
                .FirstOrDefault();

            var permissions = _context.Permissions
                .OrderBy(p => p.PermissionKey)
                .Select(p => new PermissionViewModel
                {
                    PermissionKey = p.PermissionKey,
                    Description = p.Description
                })
                .ToList();

            var parentMenus = _context.Menus
                .Where(m => m.IsActive && m.MenuId != id)
                .OrderBy(m => m.SortOrder)
                .Select(m => new MenuViewModel
                {
                    MenuId = m.MenuId,
                    ParentId = m.ParentId,
                    Title = _context.MenuTranslations
                        .Where(mt => mt.MenuId == m.MenuId && mt.LanguageId == languageId)
                        .Select(mt => mt.Title)
                        .FirstOrDefault() ?? "No Translation"
                })
                .ToList();

            // Build Tree và tính Level
            var availableParents = BuildMenuTree(parentMenus);

            var model = new MenuViewModel
            {
                MenuId = menu.MenuId,
                Url = menu.Url,
                SortOrder = menu.SortOrder,
                PermissionKey = menu.PermissionKey,
                IsActive = menu.IsActive,
                ParentId = menu.ParentId,
                Icon = menu.Icon,
                Title = _context.MenuTranslations
                    .Where(mt => mt.MenuId == menu.MenuId && mt.LanguageId == languageId)
                    .Select(mt => mt.Title)
                    .FirstOrDefault() ?? "No Translation",
                AvailablePermissions = permissions,
                AvailableParents = availableParents
            };

            return View(model);
        }

        // POST: Menu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MenuViewModel model)
        {
            if (id != model.MenuId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var menu = await _context.Menus.FindAsync(id);
                if (menu == null)
                {
                    return NotFound();
                }

                menu.Url = model.Url;
                menu.SortOrder = model.SortOrder;
                menu.PermissionKey = model.PermissionKey;
                menu.IsActive = model.IsActive;
                menu.ParentId = model.ParentId;
                menu.Icon = model.Icon;
                menu.CreatedAt = DateTime.Now;

                _context.Update(menu);
                await _context.SaveChangesAsync();

                // Cập nhật bản dịch
                var languageId1 = _context.Languages
                    .Where(l => l.Code == "vi")
                    .Select(l => l.LanguageId)
                    .FirstOrDefault();
                Console.WriteLine($"id: {id}, languageId1: {languageId1}");
                var translation = await _context.MenuTranslations
                    .FirstOrDefaultAsync(mt => mt.MenuId == id && mt.LanguageId == languageId1);

                if (translation != null)
                {
                    translation.Title = model.Title;
                    _context.Update(translation);
                }
                else
                {
                    translation = new MenuTranslation
                    {
                        MenuId = id,
                        LanguageId = languageId1,
                        Title = model.Title
                    };
                    _context.Add(translation);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, lấy lại danh sách menu cha
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            var languageId = _context.Languages
                .Where(l => l.Code == languageCode)
                .Select(l => l.LanguageId)
                .FirstOrDefault();

            var menus = _context.Menus
                .Where(m => m.IsActive && m.MenuId != id)
                .OrderBy(m => m.SortOrder)
                .Select(m => new MenuViewModel
                {
                    MenuId = m.MenuId,
                    ParentId = m.ParentId,
                    Title = _context.MenuTranslations
                        .Where(mt => mt.MenuId == m.MenuId && mt.LanguageId == languageId)
                        .Select(mt => mt.Title)
                        .FirstOrDefault() ?? "No Translation"
                })
                .ToList();

            model.AvailableParents = BuildMenuTree(menus);

            return View(model);
        }

        // POST: Menu/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }

            // Xóa menu con trước (nếu có)
            var children = _context.Menus.Where(m => m.ParentId == id);
            _context.Menus.RemoveRange(children);

            // Xóa bản dịch
            var translations = _context.MenuTranslations.Where(mt => mt.MenuId == id);
            _context.MenuTranslations.RemoveRange(translations);

            // Xóa menu
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}