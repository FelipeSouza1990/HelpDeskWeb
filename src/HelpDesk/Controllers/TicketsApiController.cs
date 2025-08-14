using HelpDesk.Data;
using HelpDesk.Models;
using HelpDesk.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Controllers;

[ApiController]
[Route("api/tickets")]
//[Authorize(Roles = "Agent,Manager")]
public class TicketsApiController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<TicketsApiController> _logger;

    public TicketsApiController(AppDbContext db, ILogger<TicketsApiController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // PATCH: /api/tickets/{id}/status
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTicketStatusRequest body)
    {
        if (body is null)
            return BadRequest(new { error = "Body obrigatório." });

        var t = await _db.Tickets.FindAsync(id);
        if (t == null)
            return NotFound();

        t.Status = body.Status;
        if (body.Status == TicketStatus.Resolved)
            t.ResolvedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { ok = true, status = t.Status.ToString() });
    }

    // Opcional: rota alternativa por URL (sem body)
    // PATCH: /api/tickets/{id}/status/Resolved   ou .../2
    [HttpPatch("{id}/status/{status}")]
    public async Task<IActionResult> UpdateStatusByRoute(int id, TicketStatus status)
    {
        var t = await _db.Tickets.FindAsync(id);
        if (t == null)
            return NotFound();

        t.Status = status;
        if (status == TicketStatus.Resolved)
            t.ResolvedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { ok = true, status = t.Status.ToString() });
    }
}
