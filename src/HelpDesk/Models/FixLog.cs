namespace HelpDesk.Models;

public class FixLog
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string Engineer { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

    public Ticket? Ticket { get; set; }
}