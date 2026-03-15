using FluentValidation;

namespace ms_transferencias.Application.Commands.CreateTransfer;

public class CreateTransferValidator : AbstractValidator<CreateTransferCommand>
{
    public CreateTransferValidator()
    {
        RuleFor(x => x.CustomerId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("EL campo CustomerId no debe ser vacío.")
            .NotNull().WithMessage("EL campo CustomerId no debe ser nulo.")
            .MaximumLength(60).WithMessage("EL campo CustomerId no debe exceder los 60 caracteres.");

        RuleFor(x => x.ServiceProviderId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("EL campo ServiceProviderId no debe ser vacío.")
            .NotNull().WithMessage("EL campo ServiceProviderId no debe ser nulo.")
            .MaximumLength(60).WithMessage("EL campo ServiceProviderId no debe exceder los 60 caracteres.");

        RuleFor(x => x.PaymentMethodId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("EL campo PaymentMethodId no debe ser vacío.")
            .NotNull().WithMessage("EL campo PaymentMethodId no debe ser nulo.");

        RuleFor(x => x.Amount)
            .NotEmpty()
                .WithMessage("EL campo Amount es obligatorio")
            .GreaterThan(0)
                .WithMessage("EL campo Amount debe ser un número positivo mayor a cero.")
            .LessThan(1000000)
                .WithMessage("EL campo Amount es demasiado grande (máximo permitido: 999,999.99).")
            .Must(amount => Decimal.Round(amount, 2) == amount && amount.ToString().Replace(".", "").Replace(",", "").Length <= 18)
                .WithMessage("El monto tiene un formato decimal inválido (máximo 18 dígitos en total y 2 decimales).");
    }
}