using MESWebDev.Services;

namespace MESWebDev.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        //private readonly ILoggingService _loggingService;
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            //_loggingService = loggingService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            ILoggingService loggingService;
            try
            {
                loggingService = context.RequestServices.GetRequiredService<ILoggingService>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resolve ILoggingService.");
                await _next(context); // Proceed without logging
                return;
            }

            var actionName = $"{context.Request.Method} {context.Request.Path}";
            var createdBy = context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "Anonymous";
            var additionalDetails = $"IP: {context.Connection.RemoteIpAddress} | UserAgent: {context.Request.Headers["User-Agent"]}";

            await loggingService.LogActionAsync<object>(
             actionName: actionName,
             actionType: "ControllerAction",
             action: async () =>
             {
                 await _next(context);
                 return null; // Return a dummy value since the middleware doesn't need a return value
             },
             createdBy: createdBy,
             additionalDetails: additionalDetails
         );
        }
    }
}