using MESWebDev.Data;
using MESWebDev.DTO;
using MESWebDev.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MESWebDev.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly AppDbContext _context;

        public BaseController(AppDbContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Lấy thông tin controller và action hiện tại từ RouteData
            string controllerName = context.RouteData.Values["controller"]?.ToString() ?? "";
            string actionName = context.RouteData.Values["action"]?.ToString() ?? "";
            // Xây dựng chuỗi URL cần so sánh dựa trên giá trị của action.
            // Nếu action là "Index", chỉ dùng controller; nếu không, dùng "/Controller/Action"
            string controller = $"/{controllerName.ToLower()}";
            string currentUrl = $"/{controllerName.ToLower()}";
            if (!string.Equals(actionName, "Index", StringComparison.OrdinalIgnoreCase))
            {
                currentUrl += $"/{actionName.ToLower()}";
            }
            // Giả sử URL của Menu được lưu dưới dạng "/Controller/Action"
            // Bạn có thể điều chỉnh điều kiện tìm kiếm Menu theo ứng dụng của bạn
            var currentMenu = _context.Menus
                .Include(m => m.Parent)
                .Include(m => m.MenuTranslations)
                .Where(m => m.IsActive &&
                       m.Url.ToLower().Contains(currentUrl.ToLower()))
                .OrderBy(m => m.SortOrder)
                .FirstOrDefault();
            var currentMenu2 = _context.Menus
                .Include(m => m.Parent)
                .Include(m => m.MenuTranslations)
                .Where(m => m.IsActive &&
                       m.Url.ToLower().Contains(controller.ToLower()))
                .OrderBy(m => m.SortOrder)
                .FirstOrDefault();
            if (currentMenu == null)
            {
                if (currentMenu2 != null)
                {
                    currentMenu = currentMenu2;
                }
            }

            if (currentMenu != null)
            {
                // Xây dựng chuỗi breadcrumb từ menu hiện tại về menu gốc
                var breadcrumbs = BuildBreadcrumb(currentMenu);
                var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
                var language = _context.Languages.Where(m => m.Code == languageCode).FirstOrDefault();
                int languageId = language.LanguageId;
                // Map sang DTO, ví dụ sử dụng LanguageId = 1 (bạn có thể thay bằng giá trị phù hợp)
                //var breadcrumbDtos = (breadcrumbs ?? new List<Menu>()).Select(m => new BreadcrumbItemDto
                //{
                //    MenuId = m.MenuId,
                //    Title = m.MenuTranslations?
                //    .FirstOrDefault(t => t.LanguageId == languageId && t.MenuId == m.MenuId)?.Title
                //                ?? m.MenuTranslations?
                //                    .FirstOrDefault(t => t.MenuId == m.MenuId)?.Title
                //                ?? string.Empty,
                //    Url = m.Url
                //}).ToList(); // Bạn có thể chuyển sang List nếu cần

                var breadcrumbDtos = (breadcrumbs ?? new List<Menu>())
                 .Select(m =>
                 {
                     // Lấy bản dịch theo ngôn ngữ mong muốn
                     var titleFromLanguage = _context.MenuTranslations
                         .Where(t => t.LanguageId == languageId && t.MenuId == m.MenuId)
                         .FirstOrDefault()?.Title;

                     // Nếu không tìm thấy bản dịch theo ngôn ngữ, lấy bản dịch đầu tiên của menu này
                     var defaultTitle = _context.MenuTranslations
                         .Where(t => t.MenuId == m.MenuId)
                         .FirstOrDefault()?.Title;

                     // Kết hợp giá trị với fallback là chuỗi rỗng nếu cả hai đều null
                     var title = !string.IsNullOrEmpty(titleFromLanguage)
                         ? titleFromLanguage
                         : defaultTitle ?? string.Empty;

                     // Nếu cần, bạn có thể log hoặc debug giá trị để kiểm tra
                     // Ví dụ:
                     // Debug.WriteLine($"MenuId: {m.MenuId} - TitleFromLanguage: {titleFromLanguage}, DefaultTitle: {defaultTitle}");

                     return new BreadcrumbItemDto
                     {
                         MenuId = m.MenuId,
                         Title = title,
                         Url = m.Url
                     };
                 })
                 .ToList();

                // Gán vào ViewBag cho toàn bộ các view kế thừa
                ViewBag.Breadcrumbs = breadcrumbDtos;
            }
            else
            {
                ViewBag.Breadcrumbs = new List<BreadcrumbItemDto>();
            }
        }

        private List<Menu> BuildBreadcrumb(Menu currentMenu)
        {
            var breadcrumbs = new List<Menu>();
            while (currentMenu != null)
            {
                breadcrumbs.Add(currentMenu);
                currentMenu = currentMenu.Parent; // Giả sử dữ liệu cha đã được Include sẵn
            }
            breadcrumbs.Reverse();
            return breadcrumbs;
        }
    }
}