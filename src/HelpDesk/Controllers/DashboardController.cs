using HelpDesk.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Controllers;

//[Authorize(Roles = "Manager")]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var byStatus = await _db
            .Tickets.GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToListAsync();

        var bySeverity = await _db
            .Tickets.GroupBy(t => t.Severity)
            .Select(g => new { Severity = g.Key.ToString(), Count = g.Count() })
            .ToListAsync();

        var avgHoursToResolve = (
            await _db
                .Tickets.Where(t => t.ResolvedAt != null)
                .Select(t => new { t.CreatedAt, t.ResolvedAt })
                .ToListAsync()
        )
            .DefaultIfEmpty()
            .Average(x => x == null ? 0 : (x.ResolvedAt!.Value - x.CreatedAt).TotalHours);

        ViewBag.ByStatus = byStatus;
        ViewBag.BySeverity = bySeverity;
        ViewBag.AvgHrs = double.IsNaN(avgHoursToResolve) ? 0 : avgHoursToResolve;
        return View();
    }
}
