using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MESWebDev.Filters
{
    public class AuthorizeLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //var userId = context.HttpContext.Session.GetInt32("UserId");
            //if (!userId.HasValue)
            //{
            //    context.Result = new RedirectToActionResult("Login", "Account", null);
            //    return;
            //}
            var Username = context.HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(Username))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}