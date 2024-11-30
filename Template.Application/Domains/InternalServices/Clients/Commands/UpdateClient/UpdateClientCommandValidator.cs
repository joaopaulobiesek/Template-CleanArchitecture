using FluentValidation;
using Template.Domain;

namespace Template.Application.Domains.InternalServices.Clients.Commands.UpdateClient;

public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("O nome completo é obrigatório.")
            .MaximumLength(100).WithMessage("O nome completo pode ter no máximo 100 caracteres.");

        RuleFor(x => x.Phone)
            .Must(phone => string.IsNullOrEmpty(phone) || StringFormatter.IsValidPhoneNumber(phone))
            .WithMessage("Número de telefone inválido.")
            .MaximumLength(20).WithMessage("O número de telefone pode ter no máximo 20 caracteres.");

        RuleFor(x => x.ZipCode)
            .Must(zipCode => string.IsNullOrEmpty(zipCode) || zipCode.Length == 8)
            .WithMessage("O CEP deve ter 8 caracteres, se fornecido.")
            .MaximumLength(10).WithMessage("O CEP pode ter no máximo 10 caracteres.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("E-mail inválido.")
            .MaximumLength(254).WithMessage("O e-mail pode ter no máximo 254 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}