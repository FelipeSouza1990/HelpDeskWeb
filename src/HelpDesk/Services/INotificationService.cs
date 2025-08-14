using HelpDesk.Models;

namespace HelpDesk.Services;

public interface INotificationService
{
    Task SendTicketResolvedAsync(Ticket t);
}