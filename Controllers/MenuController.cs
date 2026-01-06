using MESWebDev.Common;
using MESWebDev.Data;
using MESWebDev.Models;
using MESWebDev.Models.Master;
using MESWebDev.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Controllers
{
    public class MenuController : BaseController
    {
        //private readonly AppDbContext _context;

        public MenuController(AppDbContext context)
            : base(context)
        {
            // _context = context;
        }

        // GET: Menu/Index
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchTerm = null)
        {
            var Username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(Username))
            {
                return RedirectToAction("Login", "Account");
            }

            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "en";
            var languageId = await _context.Master_Language
                .Where(l => l.Culture == languageCode)
                .Select(l => l.Id)
                .FirstOrDefaultAsync();

            var Auth_Master_Function = await _context.Auth_Master_User
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
                .OrderBy(m => m.Order)
                .Select(m => new FunctionModel
                {
                    Id = m.Id,
                    EnName = m.EnName,
                    ViName = m.ViName,
                    Controller = m.Controller,
                    Action = m.Action,
                    Order = m.Order,
                    ParentId = m.ParentId,
                    IconString = m.IconString,
                    IsActive = m.IsActive
                })
                .ToListAsync();

            var menuTree = BuildMenuTree(Auth_Master_Function);

            // Không phân trang ở đây (hoặc bạn tự paging bằng js nếu muốn sau này)
            //var pagedAuth_Master_Function = new PagedResult<FunctionModel>
            //{
            //    Items = menuTree, // Full cây
            //    CurrentPage = page,
            //    PageSize = pageSize,
            //    TotalItems = menuTree.Count
            //};

            return View(menuTree);
        }

        private List<FunctionModel> BuildMenuTree(List<FunctionModel> Auth_Master_Function, int? parentId = null, int level = 0)
        {
            var rootAuth_Master_Function = Auth_Master_Function
                   .Where(m => m.ParentId == parentId)
                   .OrderBy(m => m.Order)
                   .ToList();

            foreach (var menu in rootAuth_Master_Function)
            {
                menu.Level = level;
                menu.Children = BuildMenuTree(Auth_Master_Function, menu.Id, level + 1); // đệ quy cấp con
            }
            return rootAuth_Master_Function;
        }

        // GET: Menu/Create
        public IActionResult Create()
        {
            var Username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(Username))
            {
                return RedirectToAction("Login", "Account");
            }

            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            var languageId = _context.Master_Language
                .Where(l => l.Culture == languageCode)
                .Select(l => l.Id)
                .FirstOrDefault();

            // Lấy danh sách menu cha
            var Auth_Master_Function = _context.Auth_Master_Function
                .Where(m => m.IsActive)
                .OrderBy(m => m.Order)
                .Select(m => new FunctionModel
                {
                    Id = m.Id,
                    ParentId = m.ParentId,
                    EnName = _context.Master_Language_Dic
                        .Where(mt => mt.Key == m.Id.ToString() && mt.LangId == languageId)
                        .Select(mt => mt.Value)
                        .FirstOrDefault() ?? "No Translation"
                })
                .ToList();

            // Xây dựng cây menu cha
            var availableParents = BuildMenuTree(Auth_Master_Function);

            var model = new FunctionModel
            {
                AvailableParents = availableParents
                ,IsActive = true
            };

            return View(model);
        }

        // POST: Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FunctionModel model)
        {
            if (ModelState.IsValid)
            {
                var menu = new FunctionModel
                {
                    Order = model.Order,
                    EnName = model.EnName,
                    ViName = model.ViName,
                    ParentId = model.ParentId,
                    Controller = model.Controller,
                    Action = model.Action,
                    IconString = model.IconString,
                    Note = model.Note,
                    IsActive = model.IsActive,
                    CreatedDt = DateTime.Now,
                    CreatedBy = HttpContext.Session.GetString("Username")
                };

                await _context.Auth_Master_Function.AddAsync(menu);
                await _context.SaveChangesAsync();

                // Thêm bản dịch mặc định
                var tran = new DictionaryModel
                {
                    Key = menu.Id.ToString(),
                    LangId = _context.Master_Language
                        .Where(l => l.Culture == "vi")
                        .Select(l => l.Id)
                        .FirstOrDefault(),
                    Value = model.ViName,
                    IsActive = true,
                    CreatedDt = DateTime.Now,
                    CreatedBy = HttpContext.Session.GetString("Username")
                };
                var tran2 = new DictionaryModel
                {
                    Key = menu.Id.ToString(),
                    LangId = _context.Master_Language
                        .Where(l => l.Culture == "en")
                        .Select(l => l.Id)
                        .FirstOrDefault(),
                    Value = model.EnName,
                    IsActive = true,
                    CreatedDt = DateTime.Now,
                    CreatedBy = HttpContext.Session.GetString("Username")
                };

                await _context.Master_Language_Dic.AddAsync(tran);
                await _context.Master_Language_Dic.AddAsync(tran2);
                

                // Add to role function master
                var func_role = new RoleFuncPmsModel
                {
                    RoleId = 1,
                    FuncId = menu.Id,
                    PmsId = 1,
                    IsActive = true,
                    CreatedBy = HttpContext.Session.GetString("Username"),
                    CreatedDt = DateTime.Now
                };
                await _context.Auth_Mapping_Role_Func_Pms.AddAsync(func_role);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, lấy lại danh sách menu cha
            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
            var languageId = _context.Master_Language
                .Where(l => l.Culture == languageCode)
                .Select(l => l.Id)
                .FirstOrDefault();

            var Auth_Master_Function = _context.Auth_Master_Function
                .Where(m => m.IsActive)
                .OrderBy(m => m.Order)
                .Select(m => new FunctionModel
                {
                    Id = m.Id,
                    ParentId = m.ParentId,
                    EnName = _context.Master_Language_Dic
                        .Where(mt => mt.Key == m.Id.ToString() && mt.Id == languageId)
                        .Select(mt => mt.Value)
                        .FirstOrDefault() ?? "No Translation"
                })
                .ToList();

            model.AvailableParents = BuildMenuTree(Auth_Master_Function);
            return View(model);
        }

        // GET: Menu/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            var menu = await _context.Auth_Master_Function.FindAsync(id);
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

            var parentAuth_Master_Function = _context.Auth_Master_Function
                .Where(m => m.IsActive && m.Id != id)
                .OrderBy(m => m.Order)
                .Select(m => new FunctionModel
                {
                    Id = m.Id,
                    ParentId = m.ParentId,
                    EnName = _context.Master_Language_Dic
                        .Where(mt => mt.Key == m.Id.ToString() && mt.LangId == languageId)
                        .Select(mt => mt.Value)
                        .FirstOrDefault() ?? "No Translation"
                })
                .ToList();

            // Build Tree và tính Level
            var availableParents = BuildMenuTree(parentAuth_Master_Function);

            var model = new FunctionModel
            {
                Id = menu.Id,
                EnName = menu.EnName,
                ViName = menu.ViName,
                Order = menu.Order,
                IsActive = menu.IsActive,
                ParentId = menu.ParentId,
                IconString = menu.IconString,
                Controller = menu.Controller,
                Action = menu.Action,
                Note = menu.Note,
                AvailableParents = availableParents
            };

            return View(model);
        }

        // POST: Menu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FunctionModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var menu = await _context.Auth_Master_Function.FindAsync(id);
                if (menu == null)
                {
                    return NotFound();
                }

                menu.Order = model.Order;
                menu.EnName = model.EnName;
                menu.ViName = model.ViName;
                menu.Controller = model.Controller;
                menu.Action = model.Action;
                menu.Note = model.Note;
                menu.AvailableParents = model.AvailableParents;
                menu.IconString = model.IconString;
                menu.IsActive = model.IsActive;
                menu.ParentId = model.ParentId;
                menu.UpdatedBy = HttpContext.Session.GetString("Username");
                menu.UpdatedDt = DateTime.Now;
                _context.Update(menu);
                await _context.SaveChangesAsync();

                // Cập nhật bản dịch
                var languageId1 = _context.Master_Language
                    .Where(l => l.Culture == "vi")
                    .Select(l => l.Id)
                    .FirstOrDefault();
                Console.WriteLine($"id: {id}, languageId1: {languageId1}");

                var translation = await _context.Master_Language_Dic
                    .FirstOrDefaultAsync(mt => mt.Key == id.ToString() && mt.LangId == languageId1);

                if (translation != null)
                {
                    translation.Value = model.ViName;
                    _context.Update(translation);
                }
                else
                {
                    translation = new DictionaryModel
                    {
                        Key = id.ToString(),
                        LangId = languageId1,
                        Value = model.ViName
                    };
                    _context.Add(translation);
                }

                var languageId2 = _context.Master_Language
                    .Where(l => l.Culture == "en")
                    .Select(l => l.Id)
                    .FirstOrDefault();

                var translation2 = await _context.Master_Language_Dic
                    .FirstOrDefaultAsync(mt => mt.Key == id.ToString() && mt.LangId == languageId2);

                if (translation2 != null)
                {
                    translation2.Value = model.EnName;
                    _context.Update(translation2);
                }
                else
                {
                    translation2 = new DictionaryModel
                    {
                        Key = id.ToString(),
                        LangId = languageId2,
                        Value = model.EnName
                    };
                    _context.Add(translation2);
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

            var Auth_Master_Function = _context.Auth_Master_Function
                .Where(m => m.IsActive && m.Id != id)
                .OrderBy(m => m.Order)
                .Select(m => new FunctionModel
                {
                    Id = m.Id,
                    ParentId = m.ParentId,
                    EnName = _context.Master_Language_Dic
                        .Where(mt => mt.Value == m.Id.ToString() && mt.LangId == languageId)
                        .Select(mt => mt.Value)
                        .FirstOrDefault() ?? "No Translation"
                })
                .ToList();

            model.AvailableParents = BuildMenuTree(Auth_Master_Function);

            return View(model);
        }

        // POST: Menu/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var menu = await _context.Auth_Master_Function.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }

            // Xóa menu con trước (nếu có)
            var children = _context.Auth_Master_Function.Where(m => m.ParentId == id);
            _context.Auth_Master_Function.RemoveRange(children);

            // Xóa bản dịch
            var translations = _context.Master_Language_Dic.Where(mt => mt.Key == id.ToString());
            _context.Master_Language_Dic.RemoveRange(translations);

            // Xóa menu
            _context.Auth_Master_Function.Remove(menu);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}