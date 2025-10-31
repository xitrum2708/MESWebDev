using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models;
using MESWebDev.Models.VM;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Controllers
{
    public class UserController2 : BaseController
    {
        //private readonly AppDbContext _context;
        private readonly ITranslationService _translationService;

        public UserController2(AppDbContext context, ITranslationService translationService)
            : base(context)
        {
            // _context = context;
            _translationService = translationService;
        }

        // GET: User/Index
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchTerm = null)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";

            var usersQuery = _context.Users
                .Include(u => u.Language)
                .Select(u => new UserViewModel
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    FullName = u.FullName,
                    LanguageId = u.LanguageId,
                    LanguageName = u.Language != null ? u.Language.Name : "N/A",
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                });

            if (!string.IsNullOrEmpty(searchTerm))
            {
                usersQuery = usersQuery.Where(u => u.Username.Contains(searchTerm) || u.Email.Contains(searchTerm) || u.FullName.Contains(searchTerm));
            }

            var pagedUsers = usersQuery.ToPagedResult(page, pageSize, searchTerm);

            return View(pagedUsers);
        }

        // GET: User/Create
        public async Task<IActionResult> Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Languages = await _context.Languages
                .Where(l => l.IsActive)
                .Select(l => new { l.LanguageId, l.Name })
                .ToListAsync();

            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }
            ModelState.Remove("LanguageName");
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Username = model.Username,
                    Password = model.Password, // In a real app, hash the password
                    Email = model.Email,
                    FullName = model.FullName,
                    LanguageId = model.LanguageId,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Languages = await _context.Languages
                .Where(l => l.IsActive)
                .Select(l => new { l.LanguageId, l.Name })
                .ToListAsync();

            return View(model);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                LanguageId = user.LanguageId,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            ViewBag.Languages = await _context.Languages
                .Where(l => l.IsActive)
                .Select(l => new { l.LanguageId, l.Name })
                .ToListAsync();

            return View(model);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != model.UserId)
            {
                return NotFound();
            }
            ModelState.Remove("LanguageName");
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.Users.FindAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    user.Username = model.Username;
                    user.Email = model.Email;
                    user.FullName = model.FullName;
                    user.LanguageId = model.LanguageId;
                    user.IsActive = model.IsActive;

                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        user.Password = model.Password; // In a real app, hash the password
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Languages = await _context.Languages
                .Where(l => l.IsActive)
                .Select(l => new { l.LanguageId, l.Name })
                .ToListAsync();

            return View(model);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.Include(u => u.Language).FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                LanguageId = user.LanguageId,
                LanguageName = user.Language != null ? user.Language.Name : "N/A",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            return View(model);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}