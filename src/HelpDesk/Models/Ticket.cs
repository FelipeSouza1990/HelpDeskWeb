namespace HelpDesk.Models;

public enum TicketStatus { Open = 0, InProgress = 1, Resolved = 2, Closed = 3 }
public enum TicketSeverity { Low = 0, Medium = 1, High = 2, Critical = 3 }

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public TicketSeverity Severity { get; set; } = TicketSeverity.Medium;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public int? EstimatedEffortHours { get; set; } // estimativa (requisito da vaga)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    public List<FixLog> FixLogs { get; set; } = new();
}