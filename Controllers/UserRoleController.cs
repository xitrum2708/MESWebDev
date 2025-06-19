using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models;
using MESWebDev.Models.VM;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Controllers
{
    public class UserRoleController : BaseController
    {
        //private readonly AppDbContext _context;
        private readonly ITranslationService _translationService;

        public UserRoleController(AppDbContext context, ITranslationService translationService)
            : base(context)
        {
            // _context = context;
            _translationService = translationService;
        }

        // GET: UserRole/Index
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchTerm = null)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";

            var userRolesQuery = _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Select(ur => new UserRoleViewModel
                {
                    UserId = ur.UserId,
                    Username = ur.User.Username,
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.RoleName,
                    AssignedAt = ur.AssignedAt
                });

            if (!string.IsNullOrEmpty(searchTerm))
            {
                userRolesQuery = userRolesQuery.Where(ur => ur.Username.Contains(searchTerm) ||
                ur.RoleName.Contains(searchTerm));
            }

            var pagedUserRoles = userRolesQuery.ToPagedResult(page, pageSize, searchTerm);

            return View(pagedUserRoles);
        }

        // GET: UserRole/Create
        public async Task<IActionResult> Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Users = await _context.Users
                .Select(u => new { u.UserId, u.Username })
                .ToListAsync();

            ViewBag.Roles = await _context.Roles
                .Select(r => new { r.RoleId, r.RoleName })
                .ToListAsync();

            return View();
        }

        // POST: UserRole/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRoleViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            ModelState.Remove("RoleName");
            ModelState.Remove("Username");
            if (ModelState.IsValid)
            {
                var userRole = new UserRole
                {
                    UserId = model.UserId,
                    RoleId = model.RoleId,
                    AssignedAt = DateTime.Now
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Users = await _context.Users
                .Select(u => new { u.UserId, u.Username })
                .ToListAsync();

            ViewBag.Roles = await _context.Roles
                .Select(r => new { r.RoleId, r.RoleName })
                .ToListAsync();

            return View(model);
        }

        // GET: UserRole/Delete
        public async Task<IActionResult> Delete(int userId, int roleId)
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (userRole == null)
            {
                return NotFound();
            }

            var model = new UserRoleViewModel
            {
                UserId = userRole.UserId,
                Username = userRole.User.Username,
                RoleId = userRole.RoleId,
                RoleName = userRole.Role.RoleName,
                AssignedAt = userRole.AssignedAt
            };

            return View(model);
        }

        // POST: UserRole/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(UserRoleViewModel model)
        {
            var userIdSession = HttpContext.Session.GetInt32("UserId");
            if (!userIdSession.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = await _context.UserRoles.Where(r => r.UserId == model.UserId && r.RoleId == model.RoleId).FirstOrDefaultAsync();
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}