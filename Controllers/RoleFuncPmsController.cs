using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models;
using MESWebDev.Models.Master;
using MESWebDev.Models.Master.DTO;
using MESWebDev.Models.VM;
using MESWebDev.Services;
using MESWebDev.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Controllers
{
    [Authorize]
    public class RoleFuncPmsController : BaseController
    {
        //private readonly AppDbContext _context;
        private readonly ITranslationService _translationService;
        private readonly IAuthService _auth;

        public RoleFuncPmsController(AppDbContext context, ITranslationService translationService, IAuthService auth)
            : base(context)
        {
            //_context = context;
            _translationService = translationService;
            _auth = auth;
        }

        // GET: RolePermission/Index
        public async Task<IActionResult> Index()
        {
            var data = await _auth.GetAllRoleFuncPmsAsync();
            return View(data);
        }

        // GET: RolePermission/Create
        public async Task<IActionResult> Create()
        {
            MasterVM mvm = new();
            mvm.FuncSL = await _auth.GetFunctionSL();
            mvm.RoleSL = await _auth.GetRoleSL();
            
            return View(mvm);
        }

        // POST: RolePermission/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MasterVM model)
        {
            string msg = await _auth.CreateRoleFuncPmsAsync(model.RoleFuncPms);
            if (string.IsNullOrEmpty(msg))
            {
                return RedirectToAction(nameof(Index));
            }
            model.RoleSL = await _auth.GetRoleSL();
            model.FuncSL = await _auth.GetFunctionSL();
            model.ErrorMsg = msg;
            ModelState.AddModelError(string.Empty, msg);

            return View(model);
        }

        // GET: Role/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            MasterVM mvm = new();
            mvm.RoleFuncPms = await _auth.GetRoleFuncPmsByIdAsync(id);
            mvm.FuncSL = await _auth.GetFunctionSL();
            mvm.RoleSL = await _auth.GetRoleSL();
            return View(mvm);
        }

        // POST: Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MasterVM model)
        {
            if (id != model.RoleFuncPms.Id)
            {
                return NotFound();
            }

            string msg = await _auth.UpdateRoleFuncPmsAsync(model.RoleFuncPms);

            if (string.IsNullOrEmpty(msg))
            {
                return RedirectToAction(nameof(Index));
            }
            model.FuncSL = await _auth.GetFunctionSL();
            model.RoleSL = await _auth.GetRoleSL();
            model.ErrorMsg = msg;
            ModelState.AddModelError(string.Empty, msg);

            return View(model);
        }

        // GET: RolePermission/Delete
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _auth.GetRoleFuncPmsByIdAsync(id);
            return View(data);
        }

        // POST: RolePermission/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var rolePermission = await _auth.DeleteRoleFuncPmsAsync(id);
            if (rolePermission)
            {
                return RedirectToAction(nameof(Index));
            }

            var data = await _auth.GetRoleFuncPmsByIdAsync(id);
            ModelState.AddModelError(string.Empty, "Cannot delete!");

            return View(data);
        }
    }
}