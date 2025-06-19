using MESWebDev.Data;
using MESWebDev.Extensions;
using MESWebDev.Models.VM;
using MESWebDev.Services;
using Microsoft.AspNetCore.Mvc;

namespace MESWebDev.Controllers
{
    public class ExecutionLogController : BaseController
    {
        //private readonly AppDbContext _context;
        private readonly ITranslationService _translationService;

        public ExecutionLogController(AppDbContext context, ITranslationService translationService)
            : base(context)
        {
            //_context = context;
            _translationService = translationService;
        }

        public async Task<IActionResult> Index(string actionType = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10)
        {
            var query = _context.ExecutionLogs.AsQueryable();

            if (!string.IsNullOrEmpty(actionType))
            {
                query = query.Where(l => l.TaskName.StartsWith(actionType + ":"));
            }

            if (startDate.HasValue)
            {
                query = query.Where(l => l.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                var endDateInclusive = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(l => l.CreatedAt <= endDateInclusive);
            }

            var pagedLogs = query
                       .OrderByDescending(l => l.CreatedAt)
                       .ToPagedResult(page, pageSize);

            var viewModel = new ExecutionLogViewModel
            {
                PagedLogs = pagedLogs,
                ActionType = actionType,
                StartDate = startDate,
                EndDate = endDate,
                ActionTypes = new List<string> { "ControllerAction", "StoredProcedure", "LinqQuery", "CustomTask", "BackgroundTask" }
            };

            return View(viewModel);
        }
    }
}