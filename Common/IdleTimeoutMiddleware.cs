using Microsoft.AspNetCore.Authentication;

namespace MESWebDev.Common
{
    public class IdleTimeoutMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TimeSpan _timeout;

        public IdleTimeoutMiddleware(RequestDelegate next)
        {
            _next = next;
            _timeout = TimeSpan.FromMinutes(SD.TimeOut);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var lastActivity = context.Session.GetString("LastActivity");

                if (lastActivity != null &&
                    DateTime.TryParse(lastActivity, out var last))
                {
                    if (DateTime.UtcNow - last > _timeout)
                    {
                        await context.SignOutAsync("CookieAuth");

                        context.Response.Redirect("/Account/Login");
                        return;
                    }
                }

                // Update activity
                context.Session.SetString(
                    "LastActivity",
                    DateTime.UtcNow.ToString("O"));
            }

            await _next(context);
        }
        

    }
}
