using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models;
using MESWebDev.Models.Master;
using MESWebDev.Models.Master.DTO;
using MESWebDev.Models.VM;
using MESWebDev.Services;
using MESWebDev.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Controllers
{
    public class RoleController : BaseController
    {
        //private readonly AppDbContext _context;
        private readonly ITranslationService _translationService;
        private readonly IAuthService _auth;

        public RoleController(AppDbContext context, ITranslationService translationService, IAuthService auth)
            : base(context)
        {
            _translationService = translationService;
            _auth = auth;
        }

        public async Task<IActionResult> Index()
        {
            var Username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(Username))
            {
                return RedirectToAction("Login", "Account");
            }

            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";

            // Query roles
            var roles = await _auth.GetAllRolesAsync();

            return View(roles);
        }

        // GET: Role/Create
        public IActionResult Create()
        {
            var Username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(Username))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleDTO model)
        {
            var Username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(Username))
            {
                return RedirectToAction("Login", "Account");
            }
            string msg = await _auth.CreateRoleAsync(model);
            if (string.IsNullOrEmpty(msg)) { 
                 return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, msg);

            return View(model);
        }

        // GET: Role/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var Username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(Username))
            {
                return RedirectToAction("Login", "Account");
            }

            var role = await _auth.GetRoleByIdAsync(id);

            return View(role);
        }

        // POST: Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoleDTO model)
        {
            var Username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(Username))
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != model.RoleId)
            {
                return NotFound();
            }

            string msg = await _auth.UpdateRoleAsync(model);

            if (string.IsNullOrEmpty(msg))
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, msg);
            return View(model);
        }

        // GET: Role/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var Username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(Username))
            {
                return RedirectToAction("Login", "Account");
            }

            var role = await _auth.GetRoleByIdAsync(id);

            return View(role);
        }

        // POST: Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(Username))
            {
                return RedirectToAction("Login", "Account");
            }

            var role = await _auth.DeleteRoleAsync(id);
            if(role)
            return RedirectToAction(nameof(Index));
            else
            {
                var roleData = await _auth.GetRoleByIdAsync(id);
                ModelState.AddModelError(string.Empty, "Cannot delete role because it is being used.");
                return View(roleData);
            }
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.RoleId == id);
        }
    }
}