using MESWebDev.Data;
using MESWebDev.DTO;
using MESWebDev.Models;
using MESWebDev.Models.Master;
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

            var data = _context.Auth_Master_Function
                .Where(m => m.IsActive &&
                       m.Controller != null &&
                       m.Controller.ToLower() == controllerName.ToLower() );
            if(data!=null && data.First().ParentId != null && data.First().ParentId == 2)
            {
                actionName= string.Empty;
            }
            // Giả sử URL của Menu được lưu dưới dạng "/Controller/Action"
            // Bạn có thể điều chỉnh điều kiện tìm kiếm Menu theo ứng dụng của bạn
            var currentMenu = _context.Auth_Master_Function.Include(m => m.Parent)
                .Where(m => m.IsActive &&
                       m.Controller == controllerName && m.Action.Contains(actionName))
                .OrderBy(m => m.Order)
                .FirstOrDefault();

            if (currentMenu != null)
            {
                // Xây dựng chuỗi breadcrumb từ menu hiện tại về menu gốc
                var breadcrumbs = BuildBreadcrumb(currentMenu);
                var languageCode = HttpContext.Session.GetString("LanguageCode") ?? "vi";
                var language = _context.Master_Language.Where(m => m.Culture == languageCode).FirstOrDefault();
                int languageId = language.Id;
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

                var breadcrumbDtos = (breadcrumbs ?? new List<FunctionModel>())
                 .Select(m =>
                 {
                     // Lấy bản dịch theo ngôn ngữ mong muốn
                     var titleFromLanguage = _context.Master_Language_Dic
                         .Where(t => t.LangId == languageId && t.Key == m.Id.ToString())
                         .FirstOrDefault()?.Value;

                     // Nếu không tìm thấy bản dịch theo ngôn ngữ, lấy bản dịch đầu tiên của menu này
                     var defaultTitle = _context.Master_Language_Dic
                         .Where(t => t.LangId == m.Id)
                         .FirstOrDefault()?.Value;

                     // Kết hợp giá trị với fallback là chuỗi rỗng nếu cả hai đều null
                     var title = !string.IsNullOrEmpty(titleFromLanguage)
                         ? titleFromLanguage
                         : defaultTitle ?? string.Empty;


                     return new BreadcrumbItemDto
                     {
                         MenuId = m.Id,
                         Title = title,
                         Url = Url.Action(m.Action,m.Controller)
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

        private List<FunctionModel> BuildBreadcrumb(FunctionModel currentMenu)
        {
            var breadcrumbs = new List<FunctionModel>();
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