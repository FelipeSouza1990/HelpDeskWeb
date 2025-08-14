using HelpDesk.Data;
using HelpDesk.Models;
using HelpDesk.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Controllers;

//[Authorize]
public class TicketsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IEstimationService _estimator;
    private readonly ILogger<TicketsController> _logger;

    public TicketsController(
        AppDbContext db,
        IEstimationService estimator,
        ILogger<TicketsController> logger
    )
    {
        _db = db;
        _estimator = estimator;
        _logger = logger;
    }

    // GET: /Tickets
    public async Task<IActionResult> Index([FromQuery] HelpDesk.ViewModels.TicketQuery q)
    {
        var qry = _db.Tickets.AsQueryable();

        if (q.Status.HasValue)
            qry = qry.Where(t => t.Status == q.Status.Value);
        if (q.Severity.HasValue)
            qry = qry.Where(t => t.Severity == q.Severity.Value);
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();
            qry = qry.Where(t =>
                EF.Functions.Like(t.Title, $"%{s}%")
                || EF.Functions.Like(t.Description, $"%{s}%")
                || EF.Functions.Like(t.CustomerEmail, $"%{s}%")
            );
        }

        var total = await qry.CountAsync();
        q.Page = Math.Max(1, q.Page);
        q.PageSize = Math.Clamp(q.PageSize, 5, 50);

        var items = await qry.OrderByDescending(t => t.CreatedAt)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .ToListAsync();

        ViewBag.Total = total;
        ViewBag.Query = q;
        return View(items);
    }

    // GET: /Tickets/Create
    public IActionResult Create() => View(new Ticket());

    // POST: /Tickets/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Ticket model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // estimativa automática simples com base em descrição e severidade
        model.EstimatedEffortHours = _estimator.EstimateHours(
            model.Description,
            complexity1to5: 3,
            (int)model.Severity
        );
        _db.Tickets.Add(model);
        await _db.SaveChangesAsync();
        TempData["msg"] = "Ticket criado com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Tickets/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var ticket = await _db.Tickets.Include(t => t.FixLogs).FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null)
            return NotFound();
        return View(ticket);
    }

    // POST: /Tickets/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Ticket model)
    {
        if (id != model.Id)
            return BadRequest();
        if (!ModelState.IsValid)
            return View(model);

        _db.Entry(model).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        TempData["msg"] = "Ticket atualizado!";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Tickets/AddLog
    [HttpPost]
    public async Task<IActionResult> AddLog(int ticketId, string engineer, string notes)
    {
        var t = await _db.Tickets.FindAsync(ticketId);
        if (t == null)
            return NotFound();

        var log = new FixLog
        {
            TicketId = ticketId,
            Engineer = engineer,
            Notes = notes,
            LoggedAt = DateTime.UtcNow,
        };
        _db.FixLogs.Add(log);
        await _db.SaveChangesAsync();
        return Ok(
            new
            {
                id = log.Id,
                engineer = log.Engineer,
                notes = log.Notes,
                loggedAt = log.LoggedAt,
            }
        );
    }
}
