namespace HelpDesk.Models;

public static class TicketStatusExtensions
{
    public static bool CanTransitionTo(this TicketStatus from, TicketStatus to) => (from, to) switch
    {
        (TicketStatus.Open, TicketStatus.InProgress) => true,
        (TicketStatus.InProgress, TicketStatus.Resolved) => true,
        (TicketStatus.Resolved, TicketStatus.Closed) => true,
        // Reabertura permitida
        (TicketStatus.Resolved, TicketStatus.InProgress) => true,
        (TicketStatus.Closed, TicketStatus.InProgress) => true,
        _ => false
    };
}