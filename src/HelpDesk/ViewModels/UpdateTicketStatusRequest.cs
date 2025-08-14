using HelpDesk.Models;

namespace HelpDesk.ViewModels;

public class UpdateTicketStatusRequest
{
    public TicketStatus Status { get; set; }
}