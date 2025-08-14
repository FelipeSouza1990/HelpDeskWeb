using HelpDesk.Models;
using HelpDesk.Validators;
using Xunit;

namespace HelpDesk.Tests;

public class TicketValidatorTests
{
    [Fact]
    public void Should_fail_when_email_invalid()
    {
        var v = new TicketValidator();
        var res = v.Validate(new Ticket { Title = "t", CustomerEmail = "bad", Description = "descricao adequada" });
        Assert.False(res.IsValid);
    }

    [Fact]
    public void Should_pass_with_valid_fields()
    {
        var v = new TicketValidator();
        var res = v.Validate(new Ticket { Title = "Login bug", CustomerEmail = "cli@example.com", Description = "Erro 500 ao logar", Severity = TicketSeverity.High });
        Assert.True(res.IsValid);
    }
}