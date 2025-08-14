using FluentValidation;
using HelpDesk.Models;

namespace HelpDesk.Validators;

public class TicketValidator : AbstractValidator<Ticket>
{
    public TicketValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Título é obrigatório")
            .MaximumLength(120).WithMessage("Máx 120 caracteres");

        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Descrição é obrigatória")
            .MinimumLength(10).WithMessage("Descreva um pouco melhor (>= 10)")
            .MaximumLength(4000);

        RuleFor(x => x.Severity)
            .IsInEnum();
    }
}