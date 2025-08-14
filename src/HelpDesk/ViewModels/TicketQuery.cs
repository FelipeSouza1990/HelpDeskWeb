using HelpDesk.Models;

namespace HelpDesk.ViewModels;

public class TicketQuery
{
    public TicketStatus? Status { get; set; }
    public TicketSeverity? Severity { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}