using HelpDesk.Models;

namespace HelpDesk.Services;

public class LoggingNotificationService : INotificationService
{
    private readonly ILogger<LoggingNotificationService> _logger;
    public LoggingNotificationService(ILogger<LoggingNotificationService> logger) => _logger = logger;

    public Task SendTicketResolvedAsync(Ticket t)
    {
        _logger.LogInformation("[Notify] Ticket #{Id} resolvido. Enviar e-mail para {Email}.", t.Id, t.CustomerEmail);
        return Task.CompletedTask;
    }
}