using AutoMapper;
using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models;
using MESWebDev.Models.Master;
using MESWebDev.Models.Master.DTO;
using MESWebDev.Models.VM;
using MESWebDev.Services;
using MESWebDev.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Controllers
{
    public class UserController : BaseController
    {
        //private readonly AppDbContext _context;
        private readonly ITranslationService _translationService;
        private readonly IMapper _map;
        private readonly IAuthService _authService;
        private readonly ILanguageService _lang;

        public UserController(AppDbContext context, ITranslationService translationService, IMapper map, IAuthService authService, ILanguageService lang)
            : base(context)
        {
            // _context = context;
            _translationService = translationService;
            _map = map;
            _authService = authService;
            _lang = lang;
        }

        // GET: User/Index
        public async Task<IActionResult> Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }            

            var usersQuery = _authService.GetAllUsersAsync().Result.AsQueryable();
            MasterVM mvm = new();
            mvm.Users = usersQuery.ToList();

            return View(mvm);
        }

        // GET: User/Create
        public async Task<IActionResult> Create()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }
            MasterVM mvm = new();
            mvm.User = new();
            mvm.LanguageSL = await _lang.GetLangSL();
            mvm.RoleSL = await _authService.GetRoleSL();
            return View(mvm);
        }
        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MasterVM model)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            string msg = await _authService.CreateUserAsync(model.User);
            if (string.IsNullOrEmpty(msg))
            {
                return RedirectToAction(nameof(Index));
            }

            model.LanguageSL = await _lang.GetLangSL();
            model.RoleSL = await _authService.GetRoleSL();
            model.ErrorMsg = msg;
            ModelState.AddModelError(string.Empty, msg);
            return View(model);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string Username)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new MasterVM();
            model.User = await _authService.GetUserByUsernameAsync(Username);

            model.LanguageSL = await _lang.GetLangSL();
            model.RoleSL = await _authService.GetRoleSL();

            return View(model);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MasterVM model)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }
            string msg = await _authService.UpdateUserAsync(model.User);
            if (string.IsNullOrEmpty(msg))
            {
                return RedirectToAction(nameof(Index));
            }
            model.LanguageSL = await _lang.GetLangSL();
            model.RoleSL = await _authService.GetRoleSL();
            model.ErrorMsg = msg;
            ModelState.AddModelError(string.Empty, msg);


            return View(model);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string Username)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new MasterVM();
            model.User = await _authService.GetUserByUsernameAsync(Username);

            model.LanguageSL = await _lang.GetLangSL();
            model.RoleSL = await _authService.GetRoleSL();
            return View(model);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string Username)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            bool del = await _authService.DeleteUserAsync(Username);

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}